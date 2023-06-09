using SquareBeam.AWS.EC2;
using SquareBeam.AWS.EC2.Models;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EC2ExampleUsage : MonoBehaviour
{
    [Serializable]
    public class EC2Credentials
    {
        public string accessKey;
        public string secretKey;
        public string region;
    }

    [SerializeField] private EC2Credentials credentials;

    [Header("Run Instance Block")]
    public Button RunInstanceButton;
    public InputField ImageIdField;
    public InputField InstancesCountField;
    public InputField InstanceTypeField;
    public Text RunInstanceOutputText;

    [Header("Operate Instance Block")]
    public InputField InstanceIdField;
    public Button TerminateInstanceButton;
    public Button StartInstanceButton;
    public Button StopInstanceButton;
    public Button DescribeInstanceButton;

    private EC2API EC2Client;

    void Awake()
    {
        RunInstanceButton.onClick.AddListener(() => StartCoroutine(OnRunInstance()));
        TerminateInstanceButton.onClick.AddListener(() => StartCoroutine(OnTerminateInstance()));

        StartInstanceButton.onClick.AddListener(() => StartCoroutine(OnStartInstance()));
        StopInstanceButton.onClick.AddListener(() => StartCoroutine(OnStopInstance()));
        DescribeInstanceButton.onClick.AddListener(() => StartCoroutine(OnDescribeInstance()));

#if DEVELOPMENT
        var credentialsFile = "ec2.json";
        var pathToCredentials = Path.Combine(Directory.GetParent(Application.dataPath).Parent.FullName, "credentials", credentialsFile);
        credentials = JsonUtility.FromJson<EC2Credentials>(File.ReadAllText(pathToCredentials));
#endif

        EC2Client = new GameObject("EC2 Client").AddComponent<EC2API>();
        EC2Client.Setup(credentials.accessKey, credentials.secretKey, credentials.region);
    }

    private IEnumerator OnRunInstance()
    {
        RunInstanceButton.interactable = false;
        yield return RunInstance();
        RunInstanceButton.interactable = true;
    }

    private IEnumerator OnTerminateInstance()
    {
        TerminateInstanceButton.interactable = false;
        yield return TerminateInstance();
        TerminateInstanceButton.interactable = true;
    }

    private IEnumerator OnStartInstance()
    {
        StartInstanceButton.interactable = false;
        yield return StartInstance();
        StartInstanceButton.interactable = true;
    }

    private IEnumerator OnStopInstance()
    {
        StopInstanceButton.interactable = false;
        yield return StopInstance();
        StopInstanceButton.interactable = true;
    }

    private IEnumerator OnDescribeInstance()
    {
        DescribeInstanceButton.interactable = false;
        yield return DescribeInstance();
        DescribeInstanceButton.interactable = true;
    }

    [ContextMenu("Wipe")]
    private void WipeDevDetails()
    {
        ImageIdField.text = "";
        InstancesCountField.text = "";
        InstanceTypeField.text = "";

        InstanceIdField.text = "";
    }

    IEnumerator RunInstance()
    {
        var request = new RunEC2Instance(ImageIdField.text, uint.Parse(InstancesCountField.text), uint.Parse(InstancesCountField.text), InstanceTypeField.text)
        {
            onDone = OnDone
        };

        void OnDone(EC2Response baseResponse)
        {
            var response = (RunEC2Response)baseResponse;


            if (response.Success)
            {
                Debug.Log($"Success: Response is {response.Result.RequestId}");
                RunInstanceOutputText.text = string.Join("\n", response.Result.Instances.Select(x => x.InstanceId));
                Debug.Log($"Success: Instance Id is {response.Result.Instances.First().InstanceId}");
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(EC2Exception))
                {
                    var exception = (EC2Exception)response.Exception;

                    Debug.LogError(exception.ToString());
                }
                else if (exceptionType == typeof(Exception))
                {
                    Debug.LogException(response.Exception);
                }
                else
                {
                    Debug.LogError($"Unsupported exception type {exceptionType}");
                }

            }

        }

        yield return EC2Client.RunInstance(request);
    }

    IEnumerator TerminateInstance()
    {
        var request = new TerminateEC2Instance(new string[] { InstanceIdField.text })
        {
            onDone = OnDone
        };


        void OnDone(EC2Response baseResponse)
        {
            var response = (TerminateEC2Response)baseResponse;

            if (response.Success)
            {
                Debug.Log($"Success: Response is {response.Result.RequestId}");
                Debug.Log($"Success: Current Status is {response.Result.Instances.First().CurrentState.Name}");
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(EC2Exception))
                {
                    var exception = (EC2Exception)response.Exception;

                    Debug.LogError(exception.ToString());
                }
                else if (exceptionType == typeof(Exception))
                {
                    Debug.LogException(response.Exception);
                }
                else
                {
                    Debug.LogError($"Unsupported exception type {exceptionType}");
                }

            }

        }

        yield return EC2Client.TerminateInstance(request);
    }

    IEnumerator StartInstance()
    {
        var request = new StartEC2Instance(new string[] { InstanceIdField.text })
        {
            onDone = OnDone
        };


        void OnDone(EC2Response baseResponse)
        {
            var response = (StartEC2Response)baseResponse;

            if (response.Success)
            {
                Debug.Log($"Success: Response is {response.Result.RequestId}");
                Debug.Log($"Success: Current Status is {response.Result.Instances.First().CurrentState.Name}");
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(EC2Exception))
                {
                    var exception = (EC2Exception)response.Exception;

                    Debug.LogError(exception.ToString());
                }
                else if (exceptionType == typeof(Exception))
                {
                    Debug.LogException(response.Exception);
                }
                else
                {
                    Debug.LogError($"Unsupported exception type {exceptionType}");
                }

            }

        }

        yield return EC2Client.StartInstance(request);
    }

    IEnumerator StopInstance()
    {
        var request = new StopEC2Instance(new string[] { InstanceIdField.text })
        {
            onDone = OnDone
        };


        void OnDone(EC2Response baseResponse)
        {
            var response = (StopEC2Response)baseResponse;

            if (response.Success)
            {
                Debug.Log($"Success: Response is {response.Result.RequestId}");
                Debug.Log($"Success: Current Status is {response.Result.Instances.First().CurrentState.Name}");
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(EC2Exception))
                {
                    var exception = (EC2Exception)response.Exception;

                    Debug.LogError(exception.ToString());
                }
                else if (exceptionType == typeof(Exception))
                {
                    Debug.LogException(response.Exception);
                }
                else
                {
                    Debug.LogError($"Unsupported exception type {exceptionType}");
                }

            }

        }

        yield return EC2Client.StopInstance(request);
    }

    IEnumerator DescribeInstance()
    {
        var request = new DescribeEC2Instance(new string[] { InstanceIdField.text })
        {
            onDone = OnDone
        };


        void OnDone(EC2Response baseResponse)
        {
            var response = (DescribeEC2Response)baseResponse;

            if (response.Success)
            {
                Debug.Log($"Success: Response is {response.Result.RequestId}");
                Debug.Log($"Success: Current Status is {response.Result.ReservationSet.First().Instances.First().InstanceState.Name}");
            }
            else
            {
                var exceptionType = response.Exception.GetType();
                Debug.LogError($"Failure: {exceptionType} was thrown!");

                if (exceptionType == typeof(EC2Exception))
                {
                    var exception = (EC2Exception)response.Exception;

                    Debug.LogError(exception.ToString());
                }
                else if (exceptionType == typeof(Exception))
                {
                    Debug.LogException(response.Exception);
                }
                else
                {
                    Debug.LogError($"Unsupported exception type {exceptionType}");
                }

            }

        }

        yield return EC2Client.DescribeInstance(request);
    }

}
