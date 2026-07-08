#!/usr/bin/env python3
"""Procedural low-poly OBJ generator for Tired Dad.
Builds recognizable meshes from geometric primitives (box/cylinder/cone/sphere),
each with named materials, exported as .obj + shared tireddad.mtl.
Y-up, metres. Characters/furniture: base at y=0. Items: centered near origin (~0.6 tall).
"""
import math, os

PALETTE = {
    "skin":     (0.94,0.78,0.63), "hair":(0.20,0.14,0.10), "shirt":(0.25,0.45,0.90),
    "pants":    (0.28,0.30,0.42), "sock":(0.90,0.90,0.92), "babyskin":(1.00,0.86,0.74),
    "onesie":   (0.98,0.80,0.88), "wood":(0.55,0.35,0.18), "woodDark":(0.40,0.25,0.12),
    "fabric":   (0.82,0.76,0.70), "fabricBlue":(0.50,0.65,0.85), "white":(0.96,0.96,0.96),
    "black":    (0.08,0.08,0.09), "red":(0.85,0.20,0.20), "green":(0.30,0.75,0.35),
    "orange":   (0.95,0.55,0.15), "yellow":(0.98,0.85,0.20), "brown":(0.45,0.28,0.15),
    "gray":     (0.55,0.55,0.58), "metal":(0.72,0.74,0.78), "coffee":(0.32,0.18,0.09),
    "sky":      (0.45,0.75,0.92), "pink":(0.98,0.65,0.72), "purple":(0.55,0.35,0.75),
    "cream":    (0.96,0.92,0.80), "glass":(0.80,0.92,0.95), "poop":(0.42,0.26,0.12),
    "tiger":    (0.95,0.60,0.20), "leaf":(0.35,0.65,0.30), "glow":(1.00,0.95,0.65),
}

def rotate(p, rx, ry, rz):
    x, y, z = p
    if rx:
        a=math.radians(rx); y,z = y*math.cos(a)-z*math.sin(a), y*math.sin(a)+z*math.cos(a)
    if ry:
        a=math.radians(ry); x,z = x*math.cos(a)+z*math.sin(a), -x*math.sin(a)+z*math.cos(a)
    if rz:
        a=math.radians(rz); x,y = x*math.cos(a)-y*math.sin(a), x*math.sin(a)+y*math.cos(a)
    return (x, y, z)

class Model:
    def __init__(self, name):
        self.name = name; self.v = []; self.faces = []
    def _add_v(self, p, center, rot):
        p = rotate(p, *rot)
        self.v.append((p[0]+center[0], p[1]+center[1], p[2]+center[2]))
        return len(self.v)
    def box(self, mtl, center, size, rot=(0,0,0)):
        hx,hy,hz = size[0]/2, size[1]/2, size[2]/2
        c = [(-hx,-hy,-hz),(hx,-hy,-hz),(hx,-hy,hz),(-hx,-hy,hz),
             (-hx,hy,-hz),(hx,hy,-hz),(hx,hy,hz),(-hx,hy,hz)]
        idx = [self._add_v(p, center, rot) for p in c]
        q = [(0,1,2,3),(4,7,6,5),(0,4,5,1),(2,6,7,3),(1,5,6,2),(0,3,7,4)]
        for f in q: self.faces.append((mtl,[idx[i] for i in f]))
    def cyl(self, mtl, center, radius, height, seg=14, r_top=None, rot=(0,0,0), cap=True):
        r_top = radius if r_top is None else r_top
        hy = height/2; bot=[]; top=[]
        for i in range(seg):
            a = 2*math.pi*i/seg
            bot.append(self._add_v((radius*math.cos(a),-hy,radius*math.sin(a)), center, rot))
            top.append(self._add_v((r_top*math.cos(a), hy, r_top*math.sin(a)), center, rot))
        for i in range(seg):
            j=(i+1)%seg
            self.faces.append((mtl,[bot[i],bot[j],top[j],top[i]]))
        if cap:
            cb=self._add_v((0,-hy,0),center,rot); ct=self._add_v((0,hy,0),center,rot)
            for i in range(seg):
                j=(i+1)%seg
                self.faces.append((mtl,[cb,bot[j],bot[i]]))
                if r_top>1e-5: self.faces.append((mtl,[ct,top[i],top[j]]))
    def cone(self, mtl, center, radius, height, seg=14, rot=(0,0,0)):
        self.cyl(mtl, center, radius, height, seg=seg, r_top=0.0001, rot=rot)
    def sphere(self, mtl, center, radius, seg=14, rings=10, scale=(1,1,1), rot=(0,0,0)):
        grid=[]
        for r in range(rings+1):
            lat=math.pi*r/rings; row=[]
            for s in range(seg):
                lon=2*math.pi*s/seg
                p=(radius*math.sin(lat)*math.cos(lon)*scale[0],
                   radius*math.cos(lat)*scale[1],
                   radius*math.sin(lat)*math.sin(lon)*scale[2])
                row.append(self._add_v(p, center, rot))
            grid.append(row)
        for r in range(rings):
            for s in range(seg):
                j=(s+1)%seg
                self.faces.append((mtl,[grid[r][s],grid[r][j],grid[r+1][j],grid[r+1][s]]))
    def write(self, folder):
        os.makedirs(folder, exist_ok=True)
        path=os.path.join(folder, self.name+".obj")
        with open(path,"w") as f:
            f.write(f"# Tired Dad procedural model: {self.name}\n")
            f.write("mtllib tireddad.mtl\n"); f.write(f"o {self.name}\n")
            for x,y,z in self.v: f.write(f"v {x:.4f} {y:.4f} {z:.4f}\n")
            bymtl={}
            for mtl,face in self.faces: bymtl.setdefault(mtl,[]).append(face)
            for mtl,fl in bymtl.items():
                f.write(f"usemtl {mtl}\n")
                for face in fl: f.write("f "+" ".join(str(i) for i in face)+"\n")
        return path, len(self.v), len(self.faces)

# ============================================================ CHARACTERS
def father():
    m=Model("Father")
    m.box("pants",(0,0.35,0),(0.5,0.7,0.32))
    m.box("pants",(-0.14,0.12,0),(0.2,0.24,0.34)); m.box("pants",(0.14,0.12,0),(0.2,0.24,0.34))
    m.box("black",(-0.14,0.03,0.05),(0.22,0.1,0.4)); m.box("black",(0.14,0.03,0.05),(0.22,0.1,0.4))
    m.box("shirt",(0,0.95,0),(0.62,0.62,0.4))
    m.cyl("shirt",(-0.36,0.95,0),0.1,0.6,rot=(0,0,8)); m.cyl("shirt",(0.36,0.95,0),0.1,0.6,rot=(0,0,-8))
    m.sphere("skin",(-0.4,0.66,0),0.1); m.sphere("skin",(0.4,0.66,0),0.1)
    m.sphere("skin",(0,1.45,0),0.26)
    m.box("hair",(0,1.62,-0.02),(0.5,0.16,0.5)); m.box("hair",(0,1.5,-0.24),(0.42,0.24,0.12))
    m.box("white",(-0.1,1.45,0.24),(0.12,0.06,0.04)); m.box("white",(0.1,1.45,0.24),(0.12,0.06,0.04))
    m.box("black",(-0.1,1.44,0.26),(0.05,0.03,0.03)); m.box("black",(0.1,1.44,0.26),(0.05,0.03,0.03))
    m.box("gray",(-0.1,1.4,0.24),(0.13,0.04,0.03)); m.box("gray",(0.1,1.4,0.24),(0.13,0.04,0.03))
    return m
def baby():
    m=Model("Baby")
    m.sphere("onesie",(0,0.28,0),0.24,scale=(1,0.9,1))
    m.box("onesie",(-0.1,0.06,0),(0.12,0.14,0.16)); m.box("onesie",(0.1,0.06,0),(0.12,0.14,0.16))
    m.sphere("babyskin",(-0.26,0.3,0),0.07); m.sphere("babyskin",(0.26,0.3,0),0.07)
    m.sphere("babyskin",(0,0.62,0),0.22)
    m.box("hair",(0,0.76,0),(0.28,0.1,0.28))
    m.box("black",(-0.08,0.62,0.2),(0.04,0.04,0.03)); m.box("black",(0.08,0.62,0.2),(0.04,0.04,0.03))
    m.box("pink",(0,0.55,0.21),(0.06,0.03,0.02))
    return m
# ============================================================ FURNITURE
def bed():
    m=Model("Bed")
    m.box("wood",(0,0.18,0),(1.4,0.28,2.2)); m.box("wood",(0,0.55,-1.05),(1.5,0.7,0.14))
    m.box("wood",(0,0.4,1.05),(1.5,0.4,0.14)); m.box("white",(0,0.42,0.1),(1.32,0.2,1.9))
    m.box("fabricBlue",(0,0.5,0.35),(1.32,0.12,1.3)); m.box("white",(0,0.56,-0.7),(0.9,0.18,0.5))
    return m
def chair():
    m=Model("Chair")
    m.box("wood",(0,0.5,0),(0.6,0.1,0.6)); m.box("fabric",(0,0.56,0),(0.52,0.06,0.52))
    m.box("wood",(0,0.9,-0.27),(0.6,0.8,0.08))
    for x in (-0.26,0.26):
        m.box("woodDark",(x,0.25,0),(0.07,0.5,0.5)); m.cyl("woodDark",(x,0.05,0),0.5,0.09,rot=(90,0,0))
    m.box("wood",(-0.28,0.72,0),(0.06,0.44,0.5)); m.box("wood",(0.28,0.72,0),(0.06,0.44,0.5))
    return m
def sofa():
    m=Model("Sofa")
    m.box("fabricBlue",(0,0.3,0),(1.9,0.4,0.9)); m.box("fabricBlue",(0,0.7,-0.35),(1.9,0.6,0.24))
    m.box("fabricBlue",(-0.9,0.55,0),(0.24,0.5,0.9)); m.box("fabricBlue",(0.9,0.55,0),(0.24,0.5,0.9))
    m.box("fabric",(-0.45,0.55,0.05),(0.8,0.16,0.7)); m.box("fabric",(0.45,0.55,0.05),(0.8,0.16,0.7))
    return m
def changing_table():
    m=Model("ChangingTable")
    m.box("wood",(0,0.9,0),(1.1,0.12,0.7)); m.box("cream",(0,0.99,0),(1.0,0.1,0.6))
    m.box("cream",(0,1.02,-0.28),(1.0,0.16,0.08)); m.box("cream",(-0.5,1.02,0),(0.08,0.16,0.6))
    m.box("cream",(0.5,1.02,0),(0.08,0.16,0.6))
    for x in (-0.5,0.5):
        for z in (-0.3,0.3): m.box("woodDark",(x,0.42,z),(0.08,0.84,0.08))
    m.box("wood",(0,0.35,0),(1.0,0.1,0.6))
    return m
# ============================================================ ITEMS (30)
def _note(m, mtl, cx):
    m.sphere(mtl,(cx-0.05,-0.12,0),0.1,scale=(1.2,0.9,1))
    m.box(mtl,(cx+0.04,0.05,0),(0.04,0.34,0.04)); m.box(mtl,(cx+0.12,0.2,0),(0.16,0.06,0.04))
def coffee():
    m=Model("Item_Coffee")
    m.cyl("white",(0,-0.05,0),0.18,0.34); m.cyl("coffee",(0,0.1,0),0.15,0.05)
    m.cyl("white",(0.22,-0.05,0),0.07,0.16,rot=(0,0,90)); m.box("brown",(0,-0.26,0),(0.3,0.05,0.3))
    return m
def awake_anyway():
    m=Model("Item_AwakeAnyway")
    m.sphere("white",(0,0,0),0.26,scale=(1.3,0.8,0.6)); m.sphere("sky",(0,0,0.14),0.1); m.sphere("black",(0,0,0.2),0.05)
    return m
def almost_asleep():
    m=Model("Item_AlmostAsleep")
    for x,y,s in [(-0.18,-0.05,0.14),(0.05,0.08,0.1),(0.24,0.2,0.07)]:
        m.box("sky",(x,y+s,0),(2*s,s*0.35,s*0.35)); m.box("sky",(x,y-s,0),(2*s,s*0.35,s*0.35))
        m.box("sky",(x,y,0),(s*0.4,2.4*s,s*0.35),rot=(0,0,-40))
    return m
def cute_when_asleep():
    m=Model("Item_CuteWhenAsleep")
    m.sphere("red",(-0.1,0.06,0),0.13); m.sphere("red",(0.1,0.06,0),0.13); m.cone("red",(0,-0.16,0),0.2,0.34,rot=(0,0,180))
    return m
def smile():
    m=Model("Item_Smile")
    m.sphere("yellow",(0,0,0),0.28)
    m.box("black",(-0.1,0.08,0.24),(0.05,0.08,0.04)); m.box("black",(0.1,0.08,0.24),(0.05,0.08,0.04))
    for i in range(5):
        a=math.radians(200+i*35); m.sphere("black",(0.13*math.cos(a),0.13*math.sin(a)-0.02,0.24),0.03)
    return m
def happy():
    m=Model("Item_Happy")
    m.sphere("yellow",(0,0,0),0.2)
    for i in range(8):
        a=math.radians(i*45); m.cone("orange",(0.3*math.cos(a),0.3*math.sin(a),0),0.05,0.16,rot=(0,0,90-i*45))
    return m
def hug():
    m=Model("Item_Hug")
    m.cyl("skin",(-0.02,0,-0.08),0.08,0.5,rot=(20,0,60)); m.cyl("shirt",(0.02,0.02,0.08),0.08,0.5,rot=(-20,0,-60))
    m.sphere("skin",(-0.24,0.14,-0.08),0.09); m.sphere("skin",(0.24,0.16,0.08),0.09)
    return m
def calm_movement():
    m=Model("Item_CalmMovement")
    m.box("wood",(0,0.02,0),(0.5,0.16,0.34)); m.box("wood",(0,0.14,-0.16),(0.5,0.24,0.06))
    m.cyl("woodDark",(0,-0.12,0),0.28,0.5,seg=16,r_top=0.28,rot=(90,0,0),cap=False)
    return m
def calm_music():
    m=Model("Item_CalmMusic"); _note(m,"green",0); return m
def lullaby():
    m=Model("Item_Lullaby"); _note(m,"purple",-0.16); _note(m,"purple",0.12); return m
def bottle():
    m=Model("Item_Bottle")
    m.cyl("glass",(0,-0.06,0),0.13,0.34); m.cyl("white",(0,0.14,0),0.1,0.08)
    m.cone("yellow",(0,0.24,0),0.06,0.12); m.box("sky",(0,-0.02,0.13),(0.03,0.2,0.02))
    return m
def pacifier():
    m=Model("Item_Pacifier")
    m.cyl("sky",(0,0,0),0.2,0.05); m.cone("yellow",(0,0.12,0),0.07,0.14); m.cyl("pink",(0,-0.16,0),0.1,0.04,r_top=0.1)
    return m
def it_is_so_late():
    m=Model("Item_ItIsSoLate")
    m.cyl("white",(0,0,0),0.26,0.08,seg=20,rot=(90,0,0)); m.cyl("metal",(0,0,0.05),0.28,0.03,seg=20,rot=(90,0,0),r_top=0.28)
    m.box("black",(0,0.06,0.06),(0.03,0.16,0.02)); m.box("black",(0.09,0,0.06),(0.18,0.03,0.02))
    return m
def cry():
    m=Model("Item_Cry")
    m.sphere("sky",(0,-0.05,0),0.18,scale=(1,1.1,1)); m.cone("sky",(0,0.2,0),0.11,0.24)
    return m
def no_sleep():
    m=Model("Item_NoSleep")
    m.sphere("red",(0,0,0),0.22,scale=(1,0.9,1)); m.cyl("metal",(0,-0.2,0),0.22,0.05,r_top=0.24)
    m.box("metal",(0,0.22,0),(0.05,0.1,0.05)); m.sphere("red",(-0.2,0.24,0),0.08); m.sphere("red",(0.2,0.24,0),0.08)
    return m
def toys():
    m=Model("Item_Toys")
    m.cyl("red",(0,0,0),0.05,0.5,rot=(90,0,0)); m.cyl("green",(0,0,0),0.05,0.5,rot=(0,0,90)); m.cyl("sky",(0,0,0),0.05,0.5)
    for ax in [(0,0,0.25),(0,0,-0.25),(0.25,0,0),(-0.25,0,0),(0,0.25,0),(0,-0.25,0)]:
        m.sphere("yellow",ax,0.07)
    return m
def tummy_hurts():
    m=Model("Item_TummyHurts")
    m.sphere("babyskin",(0,0,0),0.26); m.box("white",(0,0,0.24),(0.28,0.1,0.04)); m.box("white",(0,0,0.24),(0.1,0.28,0.04))
    m.sphere("red",(0,-0.02,0.26),0.04)
    return m
def must_be_easier():
    m=Model("Item_MustBeEasier")
    m.cyl("metal",(0,0,0),0.05,0.5,rot=(0,0,90)); m.cyl("black",(-0.28,0,0),0.16,0.14,rot=(0,0,90)); m.cyl("black",(0.28,0,0),0.16,0.14,rot=(0,0,90))
    return m
def loud_music():
    m=Model("Item_LoudMusic")
    m.box("black",(0,0,0),(0.5,0.34,0.2)); m.cyl("metal",(-0.12,0,0.11),0.1,0.06,rot=(90,0,0)); m.cyl("metal",(0.12,0,0.11),0.1,0.06,rot=(90,0,0))
    m.box("gray",(0,0.22,0),(0.3,0.05,0.05))
    return m
def cars():
    m=Model("Item_Cars")
    m.box("red",(0,0,0),(0.5,0.16,0.26)); m.box("red",(0.02,0.14,0),(0.28,0.16,0.24)); m.box("sky",(0.03,0.14,0.13),(0.2,0.1,0.02))
    for x,z in [(-0.16,0.14),(0.16,0.14),(-0.16,-0.14),(0.16,-0.14)]:
        m.cyl("black",(x,-0.09,z),0.07,0.05,rot=(90,0,0))
    return m
def barking_dogs():
    m=Model("Item_BarkingDogs")
    m.box("brown",(0,0.02,0),(0.44,0.2,0.2)); m.box("brown",(0.28,0.08,0),(0.18,0.18,0.18)); m.box("brown",(0.4,0.06,0),(0.1,0.1,0.14))
    m.box("brown",(0.3,0.2,-0.08),(0.06,0.1,0.04)); m.box("brown",(0.3,0.2,0.08),(0.06,0.1,0.04))
    for x,z in [(-0.14,0.08),(0.14,0.08),(-0.14,-0.08),(0.14,-0.08)]:
        m.box("brown",(x,-0.14,z),(0.07,0.18,0.07))
    m.cyl("brown",(-0.26,0.1,0),0.03,0.2,rot=(0,0,50))
    return m
def poop():
    m=Model("Item_Poop")
    m.sphere("poop",(0,-0.18,0),0.22,scale=(1,0.7,1)); m.sphere("poop",(0,-0.02,0),0.16,scale=(1,0.7,1)); m.sphere("poop",(0,0.12,0),0.1,scale=(1,0.7,1))
    m.box("black",(-0.05,-0.05,0.16),(0.04,0.04,0.03)); m.box("black",(0.06,-0.05,0.15),(0.04,0.04,0.03))
    return m
def hunger():
    m=Model("Item_Hunger")
    m.sphere("red",(0,0,0),0.22,scale=(1,0.95,1)); m.cyl("brown",(0,0.2,0),0.02,0.12); m.box("leaf",(0.08,0.24,0),(0.12,0.02,0.06),rot=(0,0,20))
    return m
def attention():
    m=Model("Item_Attention")
    m.box("yellow",(0,0.08,0),(0.14,0.36,0.14)); m.sphere("yellow",(0,-0.2,0),0.09)
    return m
def kuchy():
    m=Model("Item_KuchyKuchKu")
    m.box("skin",(-0.16,0,0),(0.24,0.3,0.08),rot=(0,0,10)); m.box("skin",(0.16,0,0),(0.24,0.3,0.08),rot=(0,0,-10))
    for x in (-0.22,-0.14,-0.06,0.06,0.14,0.22): m.cyl("skin",(x,0.2,0),0.03,0.14)
    return m
def sleeping_tiger():
    m=Model("Item_SleepingTiger")
    m.box("tiger",(0,0.02,0),(0.44,0.2,0.2)); m.box("tiger",(0.28,0.08,0),(0.2,0.2,0.2)); m.box("white",(0.4,0.05,0),(0.08,0.1,0.16))
    for i in range(3): m.box("black",(-0.1+i*0.14,0.13,0),(0.03,0.2,0.21))
    m.cyl("tiger",(-0.28,0.08,0),0.03,0.24,rot=(0,0,60))
    for x,z in [(-0.14,0.08),(0.14,0.08),(-0.14,-0.08),(0.14,-0.08)]: m.box("tiger",(x,-0.14,z),(0.07,0.18,0.07))
    return m
def burp():
    m=Model("Item_Burp")
    m.sphere("white",(0,0.05,0),0.24,scale=(1.2,0.9,0.5)); m.cone("white",(-0.12,-0.16,0),0.08,0.16,rot=(0,0,20))
    m.box("orange",(0,0.1,0.12),(0.05,0.16,0.03)); m.sphere("orange",(0,-0.02,0.12),0.03)
    return m
def work_tomorrow():
    m=Model("Item_WorkTomorrow")
    m.box("brown",(0,0,0),(0.44,0.3,0.16)); m.box("woodDark",(0,0.2,0),(0.16,0.08,0.05)); m.box("yellow",(0,0,0.09),(0.44,0.05,0.02))
    return m
def mom_is_asleep():
    m=Model("Item_MomIsAsleep")
    m.box("white",(0,-0.1,0),(0.5,0.18,0.34))
    for x,y,s in [(0.0,0.14,0.09),(0.18,0.24,0.06)]:
        m.box("sky",(x,y+s,0),(2*s,s*0.35,s*0.35)); m.box("sky",(x,y-s,0),(2*s,s*0.35,s*0.35))
        m.box("sky",(x,y,0),(s*0.4,2.4*s,s*0.35),rot=(0,0,-40))
    return m
def lights_on():
    m=Model("Item_LightsOn")
    m.sphere("glow",(0,0.05,0),0.2); m.cyl("metal",(0,-0.18,0),0.1,0.14); m.cyl("metal",(0,-0.27,0),0.07,0.06)
    return m

ITEMS = [coffee,awake_anyway,almost_asleep,cute_when_asleep,smile,happy,hug,
         calm_movement,calm_music,lullaby,bottle,pacifier,
         it_is_so_late,cry,no_sleep,toys,tummy_hurts,must_be_easier,loud_music,
         cars,barking_dogs,poop,hunger,attention,
         kuchy,sleeping_tiger,burp,work_tomorrow,mom_is_asleep,lights_on]

def write_mtl(folder):
    os.makedirs(folder, exist_ok=True)
    with open(os.path.join(folder,"tireddad.mtl"),"w") as f:
        for name,(r,g,b) in PALETTE.items():
            f.write(f"newmtl {name}\nKd {r:.3f} {g:.3f} {b:.3f}\nKa {r*0.3:.3f} {g*0.3:.3f} {b*0.3:.3f}\nKs 0.1 0.1 0.1\nNs 20\nillum 2\nd 1.0\n\n")

def run(base):
    manifest=[]
    groups={"Characters":[father,baby],"Furniture":[bed,chair,sofa,changing_table],"Items":ITEMS}
    for sub,builders in groups.items():
        folder=os.path.join(base,sub); write_mtl(folder)
        for b in builders:
            m=b(); p,nv,nf=m.write(folder); manifest.append((sub,m.name,nv,nf))
    return manifest

if __name__=="__main__":
    import sys
    base=sys.argv[1] if len(sys.argv)>1 else "Models"
    man=run(base)
    print(f"Generated {len(man)} models into {base}")
    for sub,name,nv,nf in man: print(f"  {sub:12} {name:26} v={nv:4} f={nf:4}")
