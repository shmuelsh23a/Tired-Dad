using System.Collections.Generic;
using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Code-generated set of the 30 collectibles, mapped from the original Hebrew design
    /// table (טבלת משימות.xlsx) and the old per-item scripts. Used by DemoBootstrap so the
    /// game is fully playable with no hand-authored .asset files.
    ///
    /// Convention (see ItemDefinition): a POSITIVE number RAISES that stat, NEGATIVE lowers it.
    /// Sleepiness is the goal, so raising it is good; the other four stats are bad when high.
    /// These are starting-balance values for v0.4 — tune freely.
    /// </summary>
    public static class ItemDatabase
    {
        private static readonly Color Green  = new Color(0.30f, 0.80f, 0.35f);
        private static readonly Color Red    = new Color(0.85f, 0.25f, 0.25f);
        private static readonly Color Orange = new Color(0.95f, 0.65f, 0.20f);

        public static List<ItemDefinition> Positives { get; private set; }
        public static List<ItemDefinition> Negatives { get; private set; }
        public static List<ItemDefinition> HalfAndHalf { get; private set; }

        public static void Build()
        {
            Positives = new List<ItemDefinition>
            {
                // name, He, color, father, baby, morale, needs, guilt
                Make("Coffee",        "קפה",                       ItemCategory.Positive, Green, father:-20),
                Make("Awake Anyway",  "אני כבר ער ממילא",          ItemCategory.Positive, Green, father:-12),
                Make("Almost Asleep", "הוא אוטוטו נרדם",           ItemCategory.Positive, Green, morale:-10, baby:4),
                Make("Cute When Asleep","הוא כל כך חמוד כשהוא ישן", ItemCategory.Positive, Green, morale:-12),
                Make("Smile",         "חיוך",                      ItemCategory.Positive, Green, morale:-10),
                Make("Happy",         "הוא נראה שמח",              ItemCategory.Positive, Green, guilt:-12),
                Make("Hug",           "חיבוק",                     ItemCategory.Positive, Green, guilt:-15),
                Make("Calm Movement", "נדנוד",                     ItemCategory.Positive, Green, baby:8),
                Make("Calm Music",    "מוזיקה רגועה",              ItemCategory.Positive, Green, baby:6),
                Make("Lullaby",       "שיר ערש",                   ItemCategory.Positive, Green, baby:10),
                MakeNeed("Bottle",    "בקבוק",                     ItemCategory.Positive, Green, needs:-20, clears:BabyNeed.Food),
                MakeNeed("Pacifier",  "מוצץ",                      ItemCategory.Positive, Green, needs:-15, clears:BabyNeed.Attention),
            };

            Negatives = new List<ItemDefinition>
            {
                Make("It Is So Late", "השעה כל כך מאוחרת",         ItemCategory.Negative, Red, father:15),
                Make("Cry",           "בכי",                       ItemCategory.Negative, Red, morale:15),
                Make("No Sleep",      "הוא לא נרדם",               ItemCategory.Negative, Red, morale:12),
                Make("Toys",          "צעצועים חדים על הרצפה",     ItemCategory.Negative, Red, morale:10),
                Make("Tummy Hurts",   "כואבת לו הבטן",             ItemCategory.Negative, Red, guilt:15),
                Make("Must Be Easier","לאחרים זה בטח קל יותר",     ItemCategory.Negative, Red, guilt:12),
                Make("Loud Music",    "מוזיקה רועשת",              ItemCategory.Negative, Red, baby:-12),
                Make("Cars",          "מכוניות צופרות",            ItemCategory.Negative, Red, baby:-10),
                Make("Barking Dogs",  "כלבים נובחים",              ItemCategory.Negative, Red, baby:-10),
                MakeNeed("Poop",      "קקי/פיפי",                  ItemCategory.Negative, Red, needs:15, raises:BabyNeed.Diaper),
                MakeNeed("Hunger",    "אוכל",                      ItemCategory.Negative, Red, needs:12, raises:BabyNeed.Food),
                MakeNeed("Attention", "תשומת לב",                  ItemCategory.Negative, Red, needs:12, raises:BabyNeed.Attention),
            };

            HalfAndHalf = new List<ItemDefinition>
            {
                // Mixed: one benefit + one cost each (from the "חפצים מעורבים" sheet).
                Make("Kuchy Kuch Ku", "משחק",                      ItemCategory.HalfAndHalf, Orange, morale:-10, guilt:-10, baby:-8),
                Make("Sleeping Tiger","לתפוס אותו בצורה מיוחדת",   ItemCategory.HalfAndHalf, Orange, baby:10, father:10),
                Make("Burp",          "גרעפס",                     ItemCategory.HalfAndHalf, Orange, needs:-10, baby:6, father:8),
                Make("Work Tomorrow", "מחר יש עבודה",              ItemCategory.HalfAndHalf, Orange, guilt:-8, morale:12),
                Make("Mom Is Asleep", "לפחות אמא שלו ישנה",        ItemCategory.HalfAndHalf, Orange, morale:-10, father:10),
                Make("Lights On",     "נדליק רגע אור",             ItemCategory.HalfAndHalf, Orange, father:-10, baby:-10),
            };
        }

        private static ItemDefinition Make(string name, string he, ItemCategory cat, Color color,
            float father = 0, float baby = 0, float morale = 0, float needs = 0, float guilt = 0)
        {
            var d = ScriptableObject.CreateInstance<ItemDefinition>();
            d.name = "Item_" + name.Replace(" ", "");
            d.displayName = name;
            d.displayNameHe = he;
            d.category = cat;
            d.fallbackColor = color;
            d.fatherTiredness = father;
            d.babySleepiness = baby;
            d.morale = morale;
            d.needs = needs;
            d.guilt = guilt;
            // Auto-use the .obj model if it's under a Resources/Models folder;
            // otherwise modelPrefab stays null and the spawner draws a primitive.
            d.modelPrefab = Resources.Load<GameObject>("Models/" + d.name);
            return d;
        }

        private static ItemDefinition MakeNeed(string name, string he, ItemCategory cat, Color color,
            float needs = 0, BabyNeed? clears = null, BabyNeed? raises = null)
        {
            var d = Make(name, he, cat, color, needs: needs);
            if (clears.HasValue) { d.clearsNeed = true; d.needCleared = clears.Value; }
            if (raises.HasValue) { d.raisesNeed = true; d.needRaised = raises.Value; }
            return d;
        }

        public static List<ItemDefinition> All()
        {
            var list = new List<ItemDefinition>();
            list.AddRange(Positives);
            list.AddRange(Negatives);
            list.AddRange(HalfAndHalf);
            return list;
        }
    }
}
