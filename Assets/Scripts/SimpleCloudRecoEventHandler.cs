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
    bool mIsScanning = true;
    string mTargetMetadata = "";

    string[] images = { "arbol", "balon", "bici" };
    int targetNum;
    string target;

    public GameObject[] gameObjects;

    public ImageTargetBehaviour ImageTargetTemplate;
    public Transform modelPivot;


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
        // generarNuevoTarget();
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
            ClearSpawnedObjects();
        }
    }

    string debug = "DEBUG";
    private GameObject currentInstance;
    private AssetBundle currentBundle;


    // IEnumerator GetAssetBundle(string url)
    // {
    //     UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
    //     yield return www.SendWebRequest();

    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log(www.error);
    //         debug = "" + www + "\ne: " + www.error;
    //     }
    //     else
    //     {
    //         currentBundle = DownloadHandlerAssetBundle.GetContent(www);

    //         string[] allAssetNames = currentBundle.GetAllAssetNames();
    //         string gameObjectName = Path.GetFileNameWithoutExtension(allAssetNames[0]).ToString();
    //         GameObject objectFound = currentBundle.LoadAsset<GameObject>(gameObjectName);

    //         currentInstance = Instantiate(objectFound, transform.position, transform.rotation);
    //         debug = "Descargado";
    //     }

    //     debug += "\n" + url;
    // }

    IEnumerator GetAssetBundle(string url)
    {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            currentBundle = DownloadHandlerAssetBundle.GetContent(www);
            string[] allAssetNames = currentBundle.GetAllAssetNames();
            string gameObjectName = Path.GetFileNameWithoutExtension(allAssetNames[0]);

            GameObject objectFound = currentBundle.LoadAsset<GameObject>(gameObjectName);

            // Instanciar correctamente en el pivot
            currentInstance = Instantiate(objectFound, modelPivot);

            // Ajustes locales
            currentInstance.transform.localPosition = new Vector3(0.05f, 0f, 0.05f);
            currentInstance.transform.localRotation = Quaternion.Euler(0f, 0f, -180f);
        }
    }

    private void ClearSpawnedObjects()
    {
        if (currentInstance != null)
        {
            Destroy(currentInstance);
            currentInstance = null;
        }

        if (currentBundle != null)
        {
            currentBundle.Unload(true);
            currentBundle = null;
        }
    }

    MetaDatos datos;
    public void OnNewSearchResult(CloudRecoBehaviour.CloudRecoSearchResult cloudRecoSearchResult)
    {
        datos = MetaDatos.CreateFromJSON(cloudRecoSearchResult.MetaData);

        if (datos == null) return;

        if (!GameController.controller.IsScanned(datos.nombre))
        {
            StartCoroutine(GetAssetBundle(datos.URL));
            GameController.controller.Scan(datos.nombre);
        }
        else
        {
            debug = "Imagen ya escaneada";
        }

        mCloudRecoBehaviour.enabled = false;

        if (ImageTargetTemplate)
        {
            mCloudRecoBehaviour.EnableObservers(
                cloudRecoSearchResult,
                ImageTargetTemplate.gameObject
            );
        }
    }


    void OnGUI()
    {
        // Display current 'scanning' status
        GUI.Box(new Rect(100, 100, 600, 100), mIsScanning ? "Scanning" : "Not scanning");
        // Display metadata of latest detected cloud-target
        GUI.Box(new Rect(100, 300, 600, 100), "Metadata: " + datos.nombre != null ? datos.nombre : "");
        // GUI.Box(new Rect(100, 400, 200, 100), "Busca: " + target);
        GUI.Box(new Rect(100, 700, 600, 100), "" + debug);
        // If not scanning, show button
        // so that user can restart cloud scanning
        if (!mIsScanning)
        {
            if (GUI.Button(new Rect(100, 500, 600, 100), "Restart Scanning"))
            {
                ClearSpawnedObjects();

                mCloudRecoBehaviour.enabled = true;
                mTargetMetadata = "";
                debug = "";
                mIsScanning = true;
                // generarNuevoTarget();
            }
        }
    }
}
