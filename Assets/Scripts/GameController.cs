using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController controller;

    public TextMeshProUGUI txtHint;
    private int numHint = 0;

    Dictionary<string, bool> targetsScanned = new Dictionary<string, bool>();

    string[] keys =
    {
        "Brújula Noche Estrellada",
        "Mapa Aquelarre",
        "Llave Fragua",
        "Cofre Entierro",
        "Afrodita Saturno"
    };

    string[] hints = { 
        "En el cielo siempre están para guiarnos, brillantes y copiosas, guardan algo para nuestras manos.", 
        "Todo el mundo las teme, niños y adultos por igual, pero con el pago justo el camino pueden mostrar.", 
        "Ancestral maestro de la forja y el metal, su trabajo y esfuerzo cuida al recelar.", 
        "Secretos, se revelan o se llevan a la tumba, este ahora enterró consigo lo que acaba esta trifulca.",
        "Despreciable, vil y cruel, el acto que se muestra no sólo desuella la piel"
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
        if (numHint < keys.Length && key == keys[numHint])
        {
            targetsScanned[key] = true;
            numHint++;
            ActualizarUI();
        }
    }
}
