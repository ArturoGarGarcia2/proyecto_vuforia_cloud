using UnityEngine;
using Vuforia;
using System.Collections;
using System.IO;
using UnityEngine.Networking;


public class MetaDatos
{
    public string nombre;
    public string URL;
    // public int puntuacion;

    public static MetaDatos CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<MetaDatos>(jsonString);
    }

}


public class SimpleCloudRecoEventHandler : MonoBehaviour
{
    CloudRecoBehaviour mCloudRecoBehaviour;
    bool mIsScanning = false;
    string mTargetMetadata = "";

    string[] images = { "arbol", "balon", "bici" };
    int targetNum;
    string target;

    public GameObject[] gameObjects;

    public ImageTargetBehaviour ImageTargetTemplate;

    private void generarNuevoTarget()
    {
        targetNum = Random.Range(0, images.Length + 1);
        target = images[targetNum];
    }

    // Register cloud reco callbacks
    void Awake()
    {
        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        mCloudRecoBehaviour.RegisterOnInitializedEventHandler(OnInitialized);
        mCloudRecoBehaviour.RegisterOnInitErrorEventHandler(OnInitError);
        mCloudRecoBehaviour.RegisterOnUpdateErrorEventHandler(OnUpdateError);
        mCloudRecoBehaviour.RegisterOnStateChangedEventHandler(OnStateChanged);
        mCloudRecoBehaviour.RegisterOnNewSearchResultEventHandler(OnNewSearchResult);
        generarNuevoTarget();
    }
    //Unregister cloud reco callbacks when the handler is destroyed
    void OnDestroy()
    {
        mCloudRecoBehaviour.UnregisterOnInitializedEventHandler(OnInitialized);
        mCloudRecoBehaviour.UnregisterOnInitErrorEventHandler(OnInitError);
        mCloudRecoBehaviour.UnregisterOnUpdateErrorEventHandler(OnUpdateError);
        mCloudRecoBehaviour.UnregisterOnStateChangedEventHandler(OnStateChanged);
        mCloudRecoBehaviour.UnregisterOnNewSearchResultEventHandler(OnNewSearchResult);
    }

    public void OnInitialized(CloudRecoBehaviour cloudRecoBehaviour)
    {
        Debug.Log("Cloud Reco initialized");
    }

    public void OnInitError(CloudRecoBehaviour.InitError initError)
    {
        Debug.Log("Cloud Reco init error " + initError.ToString());
    }

    public void OnUpdateError(CloudRecoBehaviour.QueryError updateError)
    {
        Debug.Log("Cloud Reco update error " + updateError.ToString());
    }

    public void OnStateChanged(bool scanning)
    {
        mIsScanning = scanning;

        if (scanning)
        {
            // Clear all known targets
        }
    }

    string debug = "";

    IEnumerator GetAssetBundle(string url) {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return www.SendWebRequest();
 
        if (www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
            debug = ""+www;
            debug += "\ne: "+www.error;
        }
        else {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            string[] allAssetNames = bundle.GetAllAssetNames();
            string gameObjectName = Path.GetFileNameWithoutExtension(allAssetNames[0]).ToString();
            GameObject objectFound = bundle.LoadAsset(gameObjectName) as GameObject;
            Instantiate(objectFound,transform.position, transform.rotation);
            debug = "Descargado";
        }
        debug += "\n"+url;
    }

    MetaDatos datos;
    // Here we handle a cloud target recognition event
    public void OnNewSearchResult(CloudRecoBehaviour.CloudRecoSearchResult cloudRecoSearchResult)
    {
        // Convertir metadatos JSON en objeto
        datos = MetaDatos.CreateFromJSON(cloudRecoSearchResult.MetaData);

        if (datos != null)
        {
            debug = datos.nombre+"Descargando figura desde URL...";
            StartCoroutine(GetAssetBundle(datos.URL));

            // Guardar nombre real del target desde JSON
            mTargetMetadata = datos.nombre;
        }
        else
        {
            debug = "Metadatos vacíos";
        }

        // Detener escaneo
        mCloudRecoBehaviour.enabled = false;

        // Activar imagen detectada
        if (ImageTargetTemplate)
        {
            mCloudRecoBehaviour.EnableObservers(cloudRecoSearchResult, ImageTargetTemplate.gameObject);
        }
    }

     GUIStyle style = new GUIStyle();

    void OnGUI()
    {
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 40;
        // Display current 'scanning' status
        GUI.Box(new Rect(100, 100, 600, 100), mIsScanning ? "Scanning" : "Not scanning", style);
        // Display metadata of latest detected cloud-target
        GUI.Box(new Rect(100, 300, 600, 100), "Metadata: " + datos.nombre != null ? datos.nombre : "", style);
        // GUI.Box(new Rect(100, 400, 200, 100), "Busca: " + target);
        // GUI.Box(new Rect(100, 700, 600, 100), "" + debug);
        // If not scanning, show button
        // so that user can restart cloud scanning
        if (!mIsScanning)
        {
            if (GUI.Button(new Rect(100, 500, 600, 100), "Restart Scanning", style))
            {
                // Reset Behaviour
                mCloudRecoBehaviour.enabled = true;
                mTargetMetadata = "";
                debug = "";
                generarNuevoTarget();
            }
        }
    }
}
