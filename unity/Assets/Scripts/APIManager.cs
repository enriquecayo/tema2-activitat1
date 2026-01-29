using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class APIManager : MonoBehaviour
{
    private const string BASE_URL = "http://localhost:3000";

    private UIDocument _doc;
    private VisualElement _root;

    private TextField _inputText;
    private TextField _outputEncrypted;
    private Label _lblDecryptedResult;

    private TextField _inputPassword;
    private TextField _outputHash;
    private Label _lblVerifyResult;

    void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _inputText = _root.Q<TextField>("input-text");
        _outputEncrypted = _root.Q<TextField>("output-encrypted");
        _lblDecryptedResult = _root.Q<Label>("lbl-decrypted-result");

        _inputPassword = _root.Q<TextField>("input-password");
        _outputHash = _root.Q<TextField>("output-hash");
        _lblVerifyResult = _root.Q<Label>("lbl-verify-result");

        _root.Q<Button>("btn-encrypt").clicked += DoEncrypt;
        _root.Q<Button>("btn-decrypt").clicked += DoDecrypt;
        _root.Q<Button>("btn-hash").clicked += DoHash;
        _root.Q<Button>("btn-verify").clicked += DoVerify;
    }

    void DoEncrypt()
    {
        string sms = _inputText.value;
        if (string.IsNullOrEmpty(sms)) return;

        string json = JsonUtility.ToJson(new RequestText { text = sms });
        StartCoroutine(EnviarPeticio("/encrypt", json, (resposta) => {
            var obj = JsonUtility.FromJson<ResponseEncrypted>(resposta);
            _outputEncrypted.value = obj.encrypted;
        }));
    }

    void DoDecrypt()
    {
        string encryptedSms = _outputEncrypted.value;
        if (string.IsNullOrEmpty(encryptedSms)) return;

        string json = JsonUtility.ToJson(new RequestEncrypted { encrypted = encryptedSms });
        StartCoroutine(EnviarPeticio("/decrypt", json, (resposta) => {
            var obj = JsonUtility.FromJson<ResponseText>(resposta);
            _lblDecryptedResult.text = "Resultat: " + obj.text;
        }));
    }

    void DoHash()
    {
        string password = _inputPassword.value;
        if (string.IsNullOrEmpty(password)) return;

        string json = JsonUtility.ToJson(new RequestPassword { password = password });
        StartCoroutine(EnviarPeticio("/hash", json, (resposta) => {
            var obj = JsonUtility.FromJson<ResponseHash>(resposta);
            _outputHash.value = obj.hash;
        }));
    }

    void DoVerify()
    {
        string password = _inputPassword.value;
        if (string.IsNullOrEmpty(password)) return;

        string json = JsonUtility.ToJson(new RequestPassword { password = password });
        StartCoroutine(EnviarPeticio("/verify", json, (resposta) => {
            var obj = JsonUtility.FromJson<ResponseVerify>(resposta);
            if (obj.ok)
            {
                _lblVerifyResult.text = "Estat: CORRECTE";
                _lblVerifyResult.style.color = Color.green;
            }
            else
            {
                _lblVerifyResult.text = "Estat: INCORRECTE";
                _lblVerifyResult.style.color = Color.red;
            }
        }));
    }

    IEnumerator EnviarPeticio(string endpoint, string jsonData, Action<string> onSuccess)
    {
        using (UnityWebRequest request = new UnityWebRequest(BASE_URL + endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }

    [Serializable] class RequestText { public string text; }
    [Serializable] class RequestEncrypted { public string encrypted; }
    [Serializable] class RequestPassword { public string password; }

    [Serializable] class ResponseText { public string text; }
    [Serializable] class ResponseEncrypted { public string encrypted; }
    [Serializable] class ResponseHash { public string hash; }
    [Serializable] class ResponseVerify { public bool ok; }
}