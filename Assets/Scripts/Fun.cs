using UnityEngine;
using System.Collections;

public class Fun
{
    public static dynamic RandomFromArray(dynamic[] array)
    {
        if (array == null)
            return null;

        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    public static dynamic WeightedRandomFromArray(dynamic[] array, float target, float weightPercent)
    {
        if (array == null)
            return null;

        target = Mathf.Clamp(target, 0, 1);
        weightPercent = Mathf.Clamp(target, 0, 1);

        float random = ((UnityEngine.Random.Range(0f, 1) * (1 - weightPercent)) + target * weightPercent) / 2;
        return array[Mathf.RoundToInt(array.Length * random)];
    }

    public static string WeightedRandomFromArray(MessagePackage mp, float target)
    {
        if (mp.messages == null)
            return null;

        target = Mathf.Clamp(target, 0, 1);
        mp.weight = Mathf.Clamp(target, 0, 1);

        float random = ((UnityEngine.Random.Range(0f, 1) * (1 - mp.weight)) + target * mp.weight) / 2;
        mp.index = Mathf.RoundToInt(mp.messages.Length * random);

        return mp.messages[mp.index];
    }

    public static string[] names = {
            //"Aerg-Tval", "Agn", "Arvant", "Belsum", "Belum", "Brint", "Börda", "Daeru",
            //"Eldar", "Felban", "Gotven", "Graft", "Grin", "Grittr", "Haerü", "Hargha",
            //"Harmug", "Jotna", "Karg", "Karva", "Katla", "Keftar", "Klort", "Kratar",
            //"Kutz", "Kvetin", "Lygan", "Margar", "Merkari", "Nagl", "Niduk", "Nifehl",
            //"Prügl", "Qillnach", "Risten", "Svind", "Theras", "Therg", "Torvul", "Törn",
            //"Urm", "Urvarg", "Vagal", "Von", "Vrakh", "Vresi", "Wemut",
            //"Connor", "Isaiah", "James", "Raphael", "Tomasz"

            "Aerg-Tval", "Agn", "Arvant", "Belsum", "Belum", "Brint", "Borda", "Daeru",
            "Eldar", "Felban", "Gotven", "Graft", "Grin", "Grittr", "Haeru", "Hargha",
            "Harmug", "Jotna", "Karg", "Karva", "Katla", "Keftar", "Klort", "Kratar",
            "Kutz", "Kvetin", "Lygan", "Margar", "Merkari", "Nagl", "Niduk", "Nifehl",
            "Prugl", "Qillnach", "Risten", "Svind", "Theras", "Therg", "Torvul", "Torn",
            "Urm", "Urvarg", "Vagal", "Von", "Vrakh", "Vresi", "Wemut",

            "Connor", "Isaiah", "James", "Raphael", "Tomasz",

            "Gilt", "Hastings", "Subar", "Osgar", "Beauner", "Edill", "Karth", "Rosse", "Kathe",
            "Arash", "Bthil", "Coln", "Django", "Etrick", "Fong", "Gozzler", "Henk", "Iglis",
            "Jeronimo", "Kong", "Loop", "Menthis", "Nort", "Orcrun", "Penelope", "Quisling",
            "Rik", "Somner", "Tzands", "Uruth", "Vader", "Wander", "Xoco", "Ygg", "Zyphon",
            "Terra", "Lucky", "Edwin", "Sabre", "Acele", "Gao", "Istrago", "Ganun", "Cyan",
            "Magenta", "Yellow", "Kblack", "Lute", "Assel", "Robo", "Mono", "Ziegfried",
            "Demos", "Riku", "Bluto", "Rhelm", "Cecil", "Kayn", "Risse", "Jane", "Mal",
            "Ashe", "Ventok", "Figaro", "Ushtar", "Quinns",
            "Shadow", "Rock", "Heavy", "Jrue", "Yan", "Portus", "Kris",
            "Baltur", "Vera", "Scorpo", "Mazer", "Raydn",
            "Herschil", "Peow", "Pao", "Qobo", "Wenth", "Erskin", "Remmy",
            "Thistle", "Ygrinth", "Ursul", "Ivank", "Owenis", "Pisto"
        };
}
