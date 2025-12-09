using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController controller;
    public TextMeshProUGUI[] txtHints;
    public TextMeshProUGUI txtHint1;
    public TextMeshProUGUI txtHint2;
    public TextMeshProUGUI txtHint3;
    public TextMeshProUGUI txtHint4;
    public TextMeshProUGUI txtFinalHint;
    public TextMeshProUGUI txtDebug;

    Dictionary <string, bool> targetsScanned = new Dictionary<string, bool>();
    string[] keys =
    {
        "Brújula Noche Estrellada",
        "Mapa Aquelarre",
        "Llave Fragua",
        "Cofre Entierro"
        // "Afrodita Saturno",
    };

    string[] originalHints = new string[4];

    private int score = 0;
    private int lives = 3;
    private string targetRequired;

    void Awake()
    {
        if (controller == null)
            controller = this;
        else
            Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // GenerarNuevoTarget();
        // ActualizarUI();
        // txtHint1.text = "<s>"+txtHint1.text+"</s>";
        foreach(string s in keys)
        {
            targetsScanned.Add(s,false);
        }
        int ite = 0;
        foreach(TextMeshProUGUI tmpu in txtHints)
        {
            originalHints[ite] = tmpu.text;
            ite++;
        }
    }

    void ActualizarUI(){
        // txtTargetName.text = "Busca: "+targetRequired;
        // txtScore.text = "Puntos: "+score;
        // txtLives.text = "Vidas: "+lives;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsScanned(string key)
    {
        txtDebug.text = "Viendo a ver si algo cambia";
        return targetsScanned[key];
    }

    public void Scan(string key)
    {
        key = key.Trim().ToLower();
        txtDebug.text = keys.Length+"";

        for (int i = 0; i < keys.Length; i++)
        {
            // txtDebug.text = txtDebug.text+keys[i]+"\n";
            if (keys[i].ToLower() == key)
            {
                targetsScanned[keys[i]] = true;
                txtHints[i].text = "<s>" + originalHints[i] + "</s>";
            }
        }
    }

    public void OnTargetFound(string targetReconocido)
    {
        // txtScore.text = targetReconocido;
        if (targetReconocido == targetRequired)
        {
            //Acierto
            score++;
        }
        else
        {
            //Fallo
            lives--;
        }
        GenerarNuevoTarget();
        if(lives < 1)
        {
            GameOver();
        }
    }

    void GenerarNuevoTarget()
    {
        //Cambiar esto por random
        // int posAleatoria = Random.Range(0, opcionesBuscar.Count);
        // targetRequired = opcionesBuscar[posAleatoria];
        ActualizarUI();
    }

    void GameOver()
    {
        // txtScore.text = "Perdiste";
        ActualizarUI();
    }
}
