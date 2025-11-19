using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI txtScore;
    public TextMeshProUGUI txtLives;
    public TextMeshProUGUI txtTargetName;

    private List<string> opcionesBuscar = new List<string>()
    {
        "balon",
        "bici"
    };

    private int score = 0;
    private int lives = 3;
    private string targetRequired;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerarNuevoTarget();
        ActualizarUI();
    }

    void ActualizarUI(){
        txtTargetName.text = "Busca: "+targetRequired;
        // txtScore.text = "Puntos: "+score;
        txtLives.text = "Vidas: "+lives;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTargetFound(string targetReconocido)
    {
        txtScore.text = targetReconocido;
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
        int posAleatoria = Random.Range(0, opcionesBuscar.Count);
        targetRequired = opcionesBuscar[posAleatoria];
        ActualizarUI();
    }

    void GameOver()
    {
        txtScore.text = "Perdiste";
        ActualizarUI();
    }
}
