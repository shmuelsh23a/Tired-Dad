#!/usr/bin/env python3
"""Fast painter's-algorithm renderer (PIL only) for the procedural models.
Produces a contact-sheet PNG with simple diffuse shading."""
import os, glob, math
from PIL import Image, ImageDraw, ImageFont

base="Assets/Models"
def load_mtl(f):
    c={};cur=None
    for ln in open(os.path.join(base,f,"tireddad.mtl")):
        if ln.startswith("newmtl"):cur=ln.split()[1]
        elif ln.startswith("Kd") and cur:c[cur]=tuple(float(x) for x in ln.split()[1:4])
    return c
def load_obj(p):
    V=[];F=[];cur="white"
    for ln in open(p):
        if ln[:2]=="v ":V.append(tuple(float(x) for x in ln.split()[1:4]))
        elif ln.startswith("usemtl"):cur=ln.split()[1]
        elif ln[:2]=="f ":F.append((cur,[int(t.split("/")[0])-1 for t in ln.split()[1:]]))
    return V,F

def rot(p,elev,azim):
    x,y,z=p
    a=math.radians(azim); x,z=x*math.cos(a)+z*math.sin(a),-x*math.sin(a)+z*math.cos(a)
    e=math.radians(elev); y,z=y*math.cos(e)-z*math.sin(e),y*math.sin(e)+z*math.cos(e)
    return (x,y,z)
def sub(a,b):return (a[0]-b[0],a[1]-b[1],a[2]-b[2])
def cross(a,b):return (a[1]*b[2]-a[2]*b[1],a[2]*b[0]-a[0]*b[2],a[0]*b[1]-a[1]*b[0])
def norm(a):
    l=math.sqrt(sum(c*c for c in a))or 1;return (a[0]/l,a[1]/l,a[2]/l)

L=norm((0.4,0.85,0.5)); ELEV,AZIM=20,35
def render_model(path,cols,size=250):
    V,F=load_obj(path)
    Vr=[rot(v,ELEV,AZIM) for v in V]
    xs=[p[0] for p in Vr];ys=[p[1] for p in Vr]
    minx,maxx,miny,maxy=min(xs),max(xs),min(ys),max(ys)
    span=max(maxx-minx,maxy-miny)*1.15 or 1
    cx=(minx+maxx)/2;cy=(miny+maxy)/2
    pad=size*0.5
    def proj(p):
        sx=(p[0]-cx)/span*size+size/2
        sy=size/2-(p[1]-cy)/span*size
        return (sx,sy)
    img=Image.new("RGBA",(size,size),(27,30,43,0))
    d=ImageDraw.Draw(img,"RGBA")
    facelist=[]
    for mtl,idx in F:
        pts3=[V[i] for i in idx]
        if len(idx)<3:continue
        n=norm(cross(sub(pts3[1],pts3[0]),sub(pts3[2],pts3[0])))
        nr=rot(n,ELEV,AZIM)
        depth=sum(Vr[i][2] for i in idx)/len(idx)
        facelist.append((depth,mtl,idx,n))
    facelist.sort(key=lambda t:t[0])  # back to front
    for depth,mtl,idx,n in facelist:
        shade=0.35+0.65*max(0,sum(a*b for a,b in zip(n,L)))
        base_c=cols.get(mtl,(0.8,0.8,0.8))
        c=tuple(int(min(1,ch*shade)*255) for ch in base_c)
        poly=[proj(Vr[i]) for i in idx]
        d.polygon(poly,fill=c+(255,),outline=(0,0,0,60))
    return img

order=[("Characters","Father"),("Characters","Baby"),("Furniture","Bed"),("Furniture","Chair"),("Furniture","Sofa"),("Furniture","ChangingTable")]
order+=[("Items",os.path.splitext(os.path.basename(p))[0]) for p in sorted(glob.glob(base+"/Items/*.obj"))]
CB={f:load_mtl(f) for f in ["Characters","Furniture","Items"]}

cell=250; ncol=6; nrow=(len(order)+ncol-1)//ncol; hdr=40; lbl=26
W=ncol*cell; H=nrow*(cell+lbl)+hdr
sheet=Image.new("RGBA",(W,H),(27,30,43,255))
d=ImageDraw.Draw(sheet)
try:font=ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",16)
except:font=ImageFont.load_default()
try:tfont=ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",22)
except:tfont=font
d.text((16,10),"Tired Dad — 36 procedural models (.obj)",fill=(235,235,240),font=tfont)
for k,(fo,nm) in enumerate(order):
    r,c=divmod(k,ncol)
    x=c*cell; y=hdr+r*(cell+lbl)
    im=render_model(f"{base}/{fo}/{nm}.obj",CB[fo],cell)
    sheet.alpha_composite(im,(x,y))
    label=nm.replace("Item_","")
    w=d.textlength(label,font=font)
    d.text((x+(cell-w)/2,y+cell-2),label,fill=(210,210,220),font=font)
out=base+"/_preview_contact_sheet.png"
sheet.convert("RGB").save(out)
print("saved",out,sheet.size)
EOF_MARKER=1
