using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController controller;

    public TextMeshProUGUI txtHint;
    public TextMeshProUGUI txtDebug;

    private int numHint = 0;

    Dictionary<string, bool> targetsScanned = new Dictionary<string, bool>();

    string[] keys =
    {
        "Brújula Noche Estrellada",
        "Mapa Aquelarre",
        "Llave Fragua",
        "Cofre Entierro"
    };

    string[] hints =
    {
        "En el cielo siempre están para guiarnos...",
        "Todo el mundo las teme...",
        "Ancestral maestro de la forja...",
        "Secretos, se revelan o se llevan a la tumba..."
    };

    void Awake()
    {
        if (controller == null)
            controller = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        for (int i = 0; i < keys.Length; i++)
            targetsScanned.Add(keys[i], false);

        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (numHint < hints.Length)
            txtHint.text = hints[numHint];
        else
            txtHint.text = "🎉 Has completado todas las pistas 🎉";
    }

    public bool IsScanned(string key)
    {
        return targetsScanned.ContainsKey(key) && targetsScanned[key];
    }

    public void Scan(string key)
    {
        txtDebug.text = "Escaneado: " + key;

        // ¿Es el target correcto?
        if (numHint < keys.Length && key == keys[numHint])
        {
            targetsScanned[key] = true;
            numHint++;
            ActualizarUI();
        }
        else
        {
            txtDebug.text += "\nNo es la imagen correcta";
        }
    }
}
