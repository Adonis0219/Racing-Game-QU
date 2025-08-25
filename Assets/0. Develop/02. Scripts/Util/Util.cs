using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class FindComponentAttribute : Attribute
{
    public string _goName { get; }

    public FindComponentAttribute(string goName)
    {
        _goName = goName;
    }
}

public class FindComponentsAttribute : Attribute
{
    public string[] _goNames { get; }

    public FindComponentsAttribute(string[] goNames)
    {
        _goNames = goNames;
    }
}


/// <summary>
/// Time, Random, Math, Enum, Json, Raycast, Log, UI, Transform, Color
/// </summary>
public class Util : MonoBehaviour
{
    public static readonly Quaternion qi = Quaternion.identity;
    public static readonly Vector3 NULL_VEC3 = new Vector3(-9999, -9999, -9999);
    public static readonly Vector2 NULL_VEC2 = new Vector2(-9999, -9999);

    #region Time

    public static readonly WaitForFixedUpdate m_waitForFixedUpdate = new WaitForFixedUpdate();
    public static readonly WaitForEndOfFrame m_waitForEndOfFrame = new WaitForEndOfFrame();

    private static readonly Dictionary<float, WaitForSeconds> m_waitForSecondsCache = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds GetWaitForSeconds(float seconds)
    {
        WaitForSeconds wfs;

        if (m_waitForSecondsCache.TryGetValue(seconds, out wfs))
        {
            return wfs;
        }
        else
        {
            wfs = new WaitForSeconds(seconds);
            m_waitForSecondsCache.Add(seconds, wfs);
            return wfs;
        }
    }

    /// <summary> format example  @"mm\:ss" </summary> 
    public static string TimeFormat(int sec, string format)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(sec);
        return timeSpan.ToString(format);
    }

    /// <summary> time format HH:MM:SS </summary>
    public static string TimeToHMS(float sec)
    {
        TimeSpan time = TimeSpan.FromSeconds(sec);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
    }

    #endregion


    #region Transform

    /// <summary>
    /// ���ϴ� ���� ������Ʈ�� Transform�� ã�� ��� �Լ�
    /// </summary>
    /// <param name="name">Ÿ�� �̸�</param>
    /// <param name="tr">���� ��ġ(���� Root)</param>
    /// <returns>ã�� Transform</returns>
    public static Transform FindChild(string name, Transform tr)
    {
        if (tr.name == name)
            return tr;

        for (int i = 0; i < tr.childCount; i++)
        {
            Transform findTr = FindChild(name, tr.GetChild(i));

            if (findTr != null)
                return findTr;
        }

        return null;
    }

    public static void DestroyAllChildImmediate(Transform parent)
    {
        while (parent.childCount > 0) MonoBehaviour.DestroyImmediate(parent.GetChild(0).gameObject);
    }

    public static void DestroyAllChild(Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static void SetLayerAllChild(Transform parent, string layerName)
    {
        parent.gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform child in parent)
        {
            SetLayerAllChild(child, layerName);
        }
    }

    public static void CopyTransform(Transform fromTransform, Transform toTransform)
    {
        toTransform.position = fromTransform.position;
        toTransform.rotation = fromTransform.rotation;
        toTransform.localScale = fromTransform.localScale;
    }

    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component component = null;
        if (destination.TryGetComponent(out T _component)) component = _component;
        else component = destination.AddComponent(type);

        foreach (var field in type.GetFields())
        {
            field.SetValue(component, field.GetValue(original));
        }
        foreach (var prop in type.GetProperties())
        {
            prop.SetValue(component, prop.GetValue(original));
        }
        return component as T;
    }

    public static void LerpDirRotation(Transform target, Vector3 dir, float lerpSpeed)
    {
        target.rotation = Quaternion.Lerp(target.rotation, Quaternion.LookRotation(dir), lerpSpeed);
    }

    #endregion


    #region Component Injector
    /// <summary>
    /// FindComponent Attribute�� ���� ������ �� ������Ʈ�� �������ִ� �Լ�
    /// </summary>
    public static void InjectComponents(object o)
    {
        Type type = o.GetType();
        MonoBehaviour script = o as MonoBehaviour;

        // static�� �ƴ� ��� �ʵ带 �����´�
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            // �ش� �ʵ忡 ������ Custom Atri�� ã�ƿ� �ʱ�ȭ �ϰ�
            var attribute = (FindComponentAttribute)field.GetCustomAttribute(typeof(FindComponentAttribute));

            // �˻�
            if (attribute == null)
            {
                Debug.Log($"�ʵ� '{field.Name}'�� ���� FindComponentAttribute�� �����ϴ�.");
                continue;   // �ش� field�� �Ѿ�� ���� field �˻�
            }

            // �ش� �ʵ��� Ÿ���� �����´� Ex) RigidBody, BoxCollider, ...
            Type fieldType = field.FieldType;
            // CA�� �����ڿ��� _gameObject�� �̸��� ���ڰ����� �̸� �ʱ�ȭ �ȴ�
            // type������ �ٷ� Transform ������ ������ �� �� ��� script ������ ����� �ʱ�ȭ
            Transform tr = FindChild(attribute._goName, script.transform);

            // �˻�
            if (tr == null)
            {
                Debug.Log($"���ӿ�����Ʈ '{attribute._goName}'�� Transform�� ã�� ���߽��ϴ�.");
                continue;
            }

            Component component = tr.GetComponent(fieldType);

            if (component == null)
            {
                Debug.Log($"���ӿ�����Ʈ '{attribute._goName}'���� '{fieldType}' ������Ʈ�� ã�� ���߽��ϴ�.");
                continue;
            }

            // script�� ���Ե� �ش� �ʵ��� ���� ã�� component�� �ʱ�ȭ ���ش�
            field.SetValue(script, component);
        }
    }

    /// <summary>
    /// FindComponent Attribute�� ���� ������ �� ������Ʈ�� �������ִ� �Լ�
    /// </summary>
    public static void InjectComponents2(object o)
    {
        Type type = o.GetType();
        MonoBehaviour script = o as MonoBehaviour;

        // static�� �ƴ� ��� �ʵ带 �����´�
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            // �ش� �ʵ忡 ������ Custom Atri�� ã�ƿ� �ʱ�ȭ �ϰ�
            var attribute = (FindComponentsAttribute)field.GetCustomAttribute(typeof(FindComponentsAttribute));

            // �˻�
            if (attribute == null)
            {
                Debug.Log($"�ʵ� '{field.Name}'�� ���� FindComponentAttribute�� �����ϴ�.");
                continue;   // �ش� field�� �Ѿ�� ���� field �˻�
            }

            // �ش� �ʵ��� Ÿ���� �����´� Ex) RigidBody, BoxCollider, ...
            Type fieldType = field.FieldType;

            // �ʵ尡 �迭�� ��� ó��
            if (fieldType.IsArray)
            {
                // �迭 ��� Ÿ�� �������� (�� : RigidBody[])
                Type elementType = fieldType.GetElementType();

                List<Component> compoList = new();

                foreach (var goName in attribute._goNames)
                {
                    Transform tr = FindChild(goName, script.transform);

                    if (tr == null)
                    {
                        Debug.Log($"���ӿ�����Ʈ '{goName}'�� Transform�� ã�� ���߽��ϴ�.");
                        continue;
                    }

                    Component component = tr.GetComponent(elementType);

                    if (component == null)
                    {
                        Debug.Log($"���ӿ�����Ʈ '{goName}'���� '{fieldType}' ������Ʈ�� ã�� ���߽��ϴ�.");
                        continue;
                    }

                    compoList.Add(component);
                }

                // ���� �迭 ���� (������ �迭 Ÿ��, ����)
                Array compoArr = Array.CreateInstance(elementType, compoList.Count);

                for (int i = 0; i < compoList.Count; i++)
                {
                    // �迭�� �� �ε����� component���� �־��ش�
                    // �־��� ���, �־��� ��
                    compoArr.SetValue(compoList[i], i);
                }

                field.SetValue(script, compoArr);
            }
            // ���� ������Ʈ ó��
            else
            {
                // CA�� �����ڿ��� _gameObject�� �̸��� ���ڰ����� �̸� �ʱ�ȭ �ȴ�
                // type������ �ٷ� Transform ������ ������ �� �� ��� script ������ ����� �ʱ�ȭ
                Transform tr = FindChild(attribute._goNames[0], script.transform);

                // �˻�
                if (tr == null)
                {
                    Debug.Log($"���ӿ�����Ʈ '{attribute._goNames[0]}'�� Transform�� ã�� ���߽��ϴ�.");
                    continue;
                }

                Component component = tr.GetComponent(fieldType);

                if (component == null)
                {
                    Debug.Log($"���ӿ�����Ʈ '{attribute._goNames[0]}'���� '{fieldType}' ������Ʈ�� ã�� ���߽��ϴ�.");
                    continue;
                }

                // script�� ���Ե� �ش� �ʵ��� ���� ã�� component�� �ʱ�ȭ ���ش�
                field.SetValue(script, component);
            }
        }
    }

    public static void InjectComponents3(object o)
    {
        Type type = o.GetType();
        MonoBehaviour script = o as MonoBehaviour;

        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var one in fields)
        {
            var attribute = (FindComponentsAttribute)one.GetCustomAttribute(typeof(FindComponentsAttribute));

            if (attribute == null)
            {
                Debug.Log($"�ʵ� '{one.Name}'�� ���� FindComponentAttribute�� �����ϴ�.");
                continue;
            }

            Type fieldType = one.FieldType;

            // �ʵ尡 �迭�� ��� ó��
            if (fieldType.IsArray)
            {
                // �迭 ��� Ÿ�� �������� (��: Rigidbody[])
                Type elementType = fieldType.GetElementType();

                List<Component> componentsList = new List<Component>();

                // ���� ���� ������Ʈ���� ������Ʈ ��������
                foreach (string gameObjectName in attribute._goNames)
                {
                    Transform tr = FindChild(gameObjectName, script.transform);

                    if (tr == null)
                    {
                        Debug.Log($"���ӿ�����Ʈ '{gameObjectName}'�� Transform�� ã�� ���߽��ϴ�.");
                        continue;
                    }

                    Component component = tr.GetComponent(elementType);
                    if (component == null)
                    {
                        Debug.Log($"���ӿ�����Ʈ '{gameObjectName}'���� '{elementType}' ������Ʈ�� ã�� ���߽��ϴ�.");
                        continue;
                    }

                    componentsList.Add(component);
                }

                Array componentArray = Array.CreateInstance(elementType, componentsList.Count);

                for (int i = 0; i < componentsList.Count; i++)
                {
                    componentArray.SetValue(componentsList[i], i);
                }

                one.SetValue(script, componentArray);
            }
            // �ʵ尡 List�� ��� ó��
            else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                // List�� ��� Ÿ�� �������� (��: List<Rigidbody>)
                Type elementType = fieldType.GetGenericArguments()[0];

                // ������Ʈ�� ������ List ����
                IList componentsList = (IList)Activator.CreateInstance(fieldType);

                foreach (string gameObjectName in attribute._goNames)
                {
                    Transform tr = FindChild(gameObjectName, script.transform);

                    if (tr == null)
                    {
                        Debug.Log($"���ӿ�����Ʈ '{gameObjectName}'�� Transform�� ã�� ���߽��ϴ�.");
                        continue;
                    }

                    Component component = tr.GetComponent(elementType);
                    if (component == null)
                    {
                        Debug.Log($"���ӿ�����Ʈ '{gameObjectName}'���� '{elementType}' ������Ʈ�� ã�� ���߽��ϴ�.");
                        continue;
                    }

                    componentsList.Add(component);
                }

                // �ʵ忡 List �Ҵ�
                one.SetValue(script, componentsList);
            }
            else
            {
                // ���� ������Ʈ ó��
                Transform tr = FindChild(attribute._goNames[0], script.transform);

                if (tr == null)
                {
                    Debug.Log($"���ӿ�����Ʈ '{attribute._goNames[0]}'�� Transform�� ã�� ���߽��ϴ�.");
                    continue;
                }

                Component component = tr.GetComponent(one.FieldType);

                if (component == null)
                {
                    Debug.Log($"���ӿ�����Ʈ '{attribute._goNames[0]}'���� '{one.FieldType}' ������Ʈ�� ã�� ���߽��ϴ�.");
                    continue;
                }

                one.SetValue(script, component);
            }
        }
    }

    public static void InjectComponents4(object o)
    {
        Type type = o.GetType();
        MonoBehaviour script = o as MonoBehaviour;

        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            var attribute = (FindComponentsAttribute)field.GetCustomAttribute(typeof(FindComponentsAttribute));

            if (attribute == null)
            {
                Debug.Log($"�ʵ� '{field.Name}'�� ���� FindComponentAttribute�� �����ϴ�.");
                continue;
            }

            // �ʵ� Ÿ�Կ� ���� �б�
            if (field.FieldType.IsArray)
            {
                InjectArrayField(field, attribute, script);
            }
            else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                InjectListField(field, attribute, script);
            }
            else
            {
                InjectSingleComponent(field, attribute, script);
            }
        }
    }

    // �迭 �ʵ� ó��
    private static void InjectArrayField(FieldInfo field, FindComponentsAttribute attribute, MonoBehaviour script)
    {
        Type elementType = field.FieldType.GetElementType();
        List<Component> componentsList = GetComponentsFromGameObjects(attribute, elementType, script);

        Array componentArray = Array.CreateInstance(elementType, componentsList.Count);

        for (int i = 0; i < componentsList.Count; i++)
        {
            componentArray.SetValue(componentsList[i], i);
        }

        field.SetValue(script, componentArray);
    }

    // ����Ʈ �ʵ� ó��
    private static void InjectListField(FieldInfo field, FindComponentsAttribute attribute, MonoBehaviour script)
    {
        Type elementType = field.FieldType.GetGenericArguments()[0];
        IList componentsList = (IList)Activator.CreateInstance(field.FieldType);

        foreach (var component in GetComponentsFromGameObjects(attribute, elementType, script))
        {
            componentsList.Add(component);
        }

        field.SetValue(script, componentsList);
    }

    // ���� ������Ʈ �ʵ� ó��
    private static void InjectSingleComponent(FieldInfo field, FindComponentsAttribute attribute, MonoBehaviour script)
    {
        Transform tr = FindChild(attribute._goNames[0], script.transform);

        if (tr == null)
        {
            Debug.Log($"���ӿ�����Ʈ '{attribute._goNames[0]}'�� Transform�� ã�� ���߽��ϴ�.");
            return;
        }

        Component component = tr.GetComponent(field.FieldType);

        if (component == null)
        {
            Debug.Log($"���ӿ�����Ʈ '{attribute._goNames[0]}'���� '{field.FieldType}' ������Ʈ�� ã�� ���߽��ϴ�.");
            return;
        }

        field.SetValue(script, component);
    }

    // ���� ���� ������Ʈ���� ������Ʈ�� �������� ���� �Լ�
    private static List<Component> GetComponentsFromGameObjects(FindComponentsAttribute attribute, Type componentType, MonoBehaviour script)
    {
        List<Component> componentsList = new List<Component>();

        foreach (string gameObjectName in attribute._goNames)
        {
            Transform tr = FindChild(gameObjectName, script.transform);

            if (tr == null)
            {
                Debug.Log($"���ӿ�����Ʈ '{gameObjectName}'�� Transform�� ã�� ���߽��ϴ�.");
                continue;
            }

            Component component = tr.GetComponent(componentType);
            if (component == null)
            {
                Debug.Log($"���ӿ�����Ʈ '{gameObjectName}'���� '{componentType}' ������Ʈ�� ã�� ���߽��ϴ�.");
                continue;
            }

            componentsList.Add(component);
        }

        return componentsList;
    }
    #endregion


    #region Random

    public static Vector3 RandomDirXY()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    public static Vector3 RandomDirXZ()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    public static T RandomPick<T>(params T[] array)
    {
        return array.OrderBy(x => UnityEngine.Random.value).First();
    }

    public static T RandomPick<T>(List<T> list)
    {
        return list.OrderBy(x => UnityEngine.Random.value).First();
    }

    public static List<T> RandomPicks<T>(int pickCount, params T[] array)
    {
        return array.OrderBy(x => UnityEngine.Random.value).Take(pickCount).ToList();
    }

    public static List<T> RandomPicks<T>(int pickCount, List<T> list)
    {
        return list.OrderBy(x => UnityEngine.Random.value).Take(pickCount).ToList();
    }

    /// <summary> random true or false </summary>
    public static bool RandomBool()
    {
        return UnityEngine.Random.Range(0, 2) == 0;
    }

    /// <summary> percent 0 ~ 100 </summary>
    public static bool RandomPercent(int percent)
    {
        return percent > UnityEngine.Random.Range(0, 100);
    }

    public static Color RandomColor()
    {
        return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
    }

    /// <summary> only hue change </summary>
    public static Color RandomColorHue()
    {
        return UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);
    }

    public static Vector3 RandomInnerPos(Vector3 pos0, Vector3 pos1)
    {
        return new Vector3(UnityEngine.Random.Range(pos0.x, pos1.x), UnityEngine.Random.Range(pos0.y, pos1.y), UnityEngine.Random.Range(pos0.z, pos1.z));
    }

    #endregion


    #region Math

    /// <summary> st, nd, rd</summary>
    public static string OrdinalNumber(int num)
    {
        if (num > 10 && num < 20) return "th";

        int remain = num % 10;
        if (remain == 1) return "st";
        else if (remain == 2) return "nd";
        else if (remain == 3) return "rd";
        else return "th";
    }

    public static float Round(float num, float digit)
    {
        return Mathf.Round(num / digit) * digit;
    }

    public static float Ceil(float num, float digit)
    {
        return Mathf.Ceil(num / digit) * digit;
    }

    public static float Floor(float num, float digit)
    {
        return Mathf.Floor(num / digit) * digit;
    }

    /// <summary> decimal point string. 1 -> 3.1, 2 -> 3.14 </summary>
    public static string DecimalPoint(float num, int unit)
    {
        return num.ToString($"N{unit}");
    }

    /// <summary> num toString fill digit multiple width </summary>
    public static string FillDigit(int num, int width, char digit = '0')
    {
        return num.ToString().PadLeft(width, digit);
    }

    public static int ParseInt(string txt, int defaultValue)
    {
        int result;
        if (!int.TryParse(txt, out result)) result = defaultValue;
        return result;
    }

    public static float ParseFloat(string txt, float defaultValue)
    {
        float result;
        if (!float.TryParse(txt, out result)) result = defaultValue;
        return result;
    }

    /// <summary> vector xy from angle 0 ~ 360 </summary>
    public static Vector3 VectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;
    }

    /// <summary> angle 0 ~ 360 from vector xy </summary>
    public static float AngleFromVector(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    /// <summary> angle 0 ~ 360 from vector xz </summary>
    public static float AngleFromVectorXZ(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    /// <summary> bring points located at the same distance from the center around circle </summary>
    public static List<Vector3> CirclePoses(Vector3 center, Vector3 axis, float distance, int count)
    {
        List<Vector3> poses = new();
        float angleStep = 360f / count;
        Vector3 forward = Vector3.forward;

        if (axis == Vector3.zero) axis = Vector3.up;
        if (axis == Vector3.forward) forward = Vector3.left;

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            Vector3 positionOffset = rotation * forward * distance;
            Vector3 position = center + positionOffset;
            poses.Add(position);
        }
        return poses;
    }

    /// <summary> bring points at the same distance from the direction center </summary>
    public static List<Vector3> DirectionLinePoses(Vector3 center, Vector3 direction, float distancePerPosition, int count)
    {
        List<Vector3> poses = new();
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = center + direction * distancePerPosition * i;
            poses.Add(pos);
        }
        return poses;
    }

    /// <summary> bring points between two poses contain end</summary>
    public static List<Vector3> BetweenLinePoses(Vector3 pos0, Vector3 pos1, int count)
    {
        List<Vector3> poses = new();
        for (int i = 0; i < count; i++)
        {
            Vector3 currentPos = Vector3.Lerp(pos0, pos1, i / (float)(count - 1));
            poses.Add(currentPos);
        }
        return poses;
    }

    /// <summary> parabola oneway step </summary>
    public static List<Vector3> ParabolaPoses(Vector3 origin, Vector3 velocity, float timeStep, float duration, float gravity = 9.8f)
    {
        int numSteps = Mathf.CeilToInt(duration / timeStep);
        List<Vector3> poses = new List<Vector3>(numSteps);
        for (int i = 0; i < numSteps; i++)
        {
            velocity += gravity * timeStep * Vector3.down;
            origin += velocity * timeStep;
            poses.Add(origin);
        }
        return poses;
    }

    /// <summary> arc two point count </summary>
    public static List<Vector3> BetweenArcCircularPoses(Vector3 pos0, Vector3 pos1, Vector3 center, int count)
    {
        List<Vector3> poses = new List<Vector3>();
        Vector3 tempCenter = (pos0 + pos1) * 0.5f;
        tempCenter = tempCenter - center;
        pos0 -= tempCenter;
        pos1 -= tempCenter;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = Vector3.Slerp(pos0, pos1, i / (float)(count - 1));
            pos += tempCenter;
            poses.Add(pos);
        }
        return poses;
    }


    #endregion


    #region Enum

    public static T RandomEnum<T>()
    {
        var enumValues = Enum.GetValues(enumType: typeof(T));
        return (T)enumValues.GetValue(UnityEngine.Random.Range(0, enumValues.Length));
    }

    public static T ToEnum<T>(string value)
    {
        if (!Enum.IsDefined(typeof(T), value))
            return default(T);

        return (T)Enum.Parse(typeof(T), value, true);
    }

    #endregion


    #region Json

    class SerializationList<T>
    {
        public SerializationList(List<T> target) => this.target = target;
        public List<T> target;
    }

    class SerializationArray<T>
    {
        public SerializationArray(T[] target) => this.target = target;
        public T[] target;
    }

    class SerializationDic<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField] List<TKey> keys;
        [SerializeField] List<TValue> values;

        Dictionary<TKey, TValue> target;
        public Dictionary<TKey, TValue> ToDictionary() => target;

        public SerializationDic(Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(target.Keys);
            values = new List<TValue>(target.Values);
        }

        public void OnAfterDeserialize()
        {
            int count = Math.Min(keys.Count, values.Count);
            target = new Dictionary<TKey, TValue>(count);
            for (int i = 0; i < count; ++i)
                target.Add(keys[i], values[i]);
        }
    }

    public static string ToJson<T>(T[] array)
    {
        return JsonUtility.ToJson(new SerializationArray<T>(array));
    }

    public static string ToJson<T>(List<T> list)
    {
        return JsonUtility.ToJson(new SerializationList<T>(list));
    }

    public static string ToJson<TKey, TValue>(Dictionary<TKey, TValue> dic)
    {
        return JsonUtility.ToJson(new SerializationDic<TKey, TValue>(dic));
    }

    public static T[] FromJsonArray<T>(string jdata)
    {
        return JsonUtility.FromJson<SerializationArray<T>>(jdata).target;
    }

    public static List<T> FromJsonList<T>(string jdata)
    {
        return JsonUtility.FromJson<SerializationList<T>>(jdata).target;
    }

    public static Dictionary<TKey, TValue> FromJsonDic<TKey, TValue>(string jdata)
    {
        return JsonUtility.FromJson<SerializationDic<TKey, TValue>>(jdata).ToDictionary();
    }

    #endregion


    #region Raycast

    /// <summary> only 3D collider, camera orthographic and perspective work </summary>
    public static bool MouseRay(out RaycastHit hit, string layerName = null, Camera camera = null)
    {
        if (camera == null) camera = Camera.main;
        bool isTouch = false;
        if (layerName == null)
        {
            isTouch = Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit curHit);
            hit = curHit;
        }
        else
        {
            isTouch = Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit curHit2,
                float.MaxValue, 1 << LayerMask.NameToLayer(layerName));
            hit = curHit2;
        }
        return isTouch;
    }

    /// <summary> only 3D collider, camera orthographic and perspective work </summary>
    public static bool MouseRayAll(out RaycastHit[] hits, string layerName = null, Camera camera = null)
    {
        if (camera == null) camera = Camera.main;
        if (layerName == null)
        {
            hits = Physics.RaycastAll(camera.ScreenPointToRay(Input.mousePosition), Mathf.Infinity);
        }
        else
        {
            hits = Physics.RaycastAll(camera.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, 1 << LayerMask.NameToLayer(layerName));
        }
        return hits.Length > 0;
    }

    /// <summary> only 2D collider, camera orthographic work </summary>
    public static bool MouseRay2D(out RaycastHit2D hit, string layerName = null, Camera camera = null)
    {
        if (camera == null) camera = Camera.main;
        if (layerName == null)
        {
            hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        }
        else
        {
            hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,
                float.MaxValue, 1 << LayerMask.NameToLayer(layerName));
        }
        return hit.collider != null;
    }

    /// <summary> only 2D collider, camera orthographic work </summary>
    public static bool MouseRay2DAll(out RaycastHit2D[] hits, string layerName = null, Camera camera = null)
    {
        if (camera == null) camera = Camera.main;
        if (layerName == null)
        {
            hits = Physics2D.RaycastAll(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        }
        else
        {
            hits = Physics2D.RaycastAll(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,
                float.MaxValue, 1 << LayerMask.NameToLayer(layerName));
        }
        return hits.Length > 0;
    }

    #endregion


    #region Log

    public static void Log<T>(T t)
    {
        string log = $"{t.GetType().Name}\n";
        log += $"raw: {t}\n";
        foreach (FieldInfo fieldInfo in t.GetType().GetFields())
        {
            log += $"{fieldInfo.Name}: {fieldInfo.GetValue(t)}\n";
        }
        foreach (PropertyInfo propertyInfo in t.GetType().GetProperties())
        {
            log += $"{propertyInfo.Name}: {propertyInfo.GetValue(t)}\n";
        }
        Debug.Log(log);
    }

    public static void Log<T>(params T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            T t = array[i];
            string log = $"{array.GetType().Name} {i}\n";
            log += $"raw: {t}\n";
            foreach (FieldInfo fieldInfo in t.GetType().GetFields())
            {
                log += $"{fieldInfo.Name}: {fieldInfo.GetValue(t)}\n";
            }
            foreach (PropertyInfo propertyInfo in t.GetType().GetProperties())
            {
                log += $"{propertyInfo.Name}: {propertyInfo.GetValue(t)}\n";
            }
            Debug.Log(log);
        }
    }

    public static void Log<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T t = list[i];
            string log = $"{list.GetType().Name} {i}\n";
            log += $"raw: {t}\n";
            foreach (FieldInfo fieldInfo in t.GetType().GetFields())
            {
                log += $"{fieldInfo.Name}: {fieldInfo.GetValue(t)}\n";
            }
            foreach (PropertyInfo propertyInfo in t.GetType().GetProperties())
            {
                log += $"{propertyInfo.Name}: {propertyInfo.GetValue(t)}\n";
            }
            Debug.Log(log);
        }
    }


    #endregion


    #region Search

    /// <summary> find the value of an instance of a class by variable name </summary>
    public static T GetInstanceValue<T>(object instance, string valueName)
    {
        return (T)instance.GetType().GetField(valueName).GetValue(instance);
    }

    #endregion


    #region Camera

    /// <summary> camera orthographic bound </summary>
    public static Bounds OrthoBounds(Camera camera = null)
    {
        if (camera == null) camera = Camera.main;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(camera.transform.position, new Vector3(cameraHeight * camera.aspect, cameraHeight, 0));
        return bounds;
    }

    /// <summary> camera orthographic mouse world position </summary>
    public static Vector3 OrthoMouseWorldPos(bool isZZero, Camera camera = null)
    {
        if (camera == null) camera = Camera.main;
        Vector3 pos = camera.ScreenToWorldPoint(Input.mousePosition);
        if (isZZero) pos.z = 0;
        return pos;
    }


    #endregion


    #region Scene

    public static void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    #endregion


    #region UI

    public static void RectFits(params RectTransform[] rects)
    {
        foreach (var rect in rects)
        {
            RectFit(rect);
        }
    }

    public static void RectFit(RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    /// <summary> canvasGroup.alpha is your share </summary>
    public static void SetCanvasActive(CanvasGroup canvasGroup, bool isActive)
    {
        canvasGroup.interactable = isActive;
        canvasGroup.blocksRaycasts = isActive;
        if (!isActive) canvasGroup.alpha = 0;
    }

    public static bool IsMouseOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        else
        {
            var eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, hits);
            return hits.Count > 0;
        }
    }

    public static bool IsMouseInBoundUI(RectTransform rect)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition);
    }

    /// <summary> only canvas render mode screen space - overlay
    /// camera orthographic and perspective work
    /// set ui object's transform.position </summary>
    public static Vector3 UIPosFromWorldPos(Vector3 worldPos, Camera camera = null)
    {
        if (camera == null) camera = Camera.main;
        return camera.WorldToScreenPoint(worldPos);
    }

    /// <summary> on update world canvas look camera </summary>
    public static void WorldCanvasLookCamera(Transform worldCanvas, Camera camera = null)
    {
        if (camera == null) camera = Camera.main;
        worldCanvas.rotation = camera.transform.rotation;
    }

    /// <summary> mouse position on canvas scaled ui coodinate. </summary>
    public static Vector2 MousePosOnCanvas(CanvasScaler canvasScaler)
    {
        return new Vector2(Input.mousePosition.x * canvasScaler.referenceResolution.x / Screen.width,
            Input.mousePosition.y * canvasScaler.referenceResolution.y / Screen.height);
    }


    #endregion


    #region Color

    public static string ColorToHex(Color color)
    {
        Color32 color32 = color;
        string hex = "#" + color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2");
        return hex;
    }

    public static Color HexToColor(string hexCode)
    {
        hexCode = hexCode.Replace("#", "");
        byte r = byte.Parse(hexCode.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hexCode.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hexCode.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte a = 255;
        Color color = new Color32(r, g, b, a);
        return color;
    }

    #endregion


    #region Enumerate

    /// <summary> shallow copy, If you don't want to touch the original use this.
    /// itemList.Where(x => x.level <= maxLevel).OrderBy(n => UnityEngine.Random.value).Take(2).ToList(); </summary>
    public static void ShuffleShallow<T>(T[] array)
    {
        int ran1, ran2;
        T tmp;

        for (int index = 0; index < array.Length; ++index)
        {
            ran1 = UnityEngine.Random.Range(0, array.Length);
            ran2 = UnityEngine.Random.Range(0, array.Length);

            tmp = array[ran1];
            array[ran1] = array[ran2];
            array[ran2] = tmp;
        }
    }

    /// <summary> shallow copy, If you don't want to touch the original use this. 
    /// itemList.Where(x => x.level <= maxLevel).OrderBy(n => UnityEngine.Random.value).Take(2).ToList(); </summary>
    public static void ShuffleShallow<T>(List<T> list)
    {
        int ran1, ran2;
        T tmp;

        for (int index = 0; index < list.Count; ++index)
        {
            ran1 = UnityEngine.Random.Range(0, list.Count);
            ran2 = UnityEngine.Random.Range(0, list.Count);

            tmp = list[ran1];
            list[ran1] = list[ran2];
            list[ran2] = tmp;
        }
    }

    /// <summary> generate min to max int list by interval </summary>
    public static List<int> Range(int min, int max, int interval = 1)
    {
        List<int> results = new();
        for (int i = min; i < max; i = i + interval)
        {
            results.Add(i);
        }
        return results;
    }

    public static void ActionEnumerate<T>(Action<T> action, params T[] values)
    {
        foreach (var value in values)
        {
            action?.Invoke(value);
        }
    }



    #endregion


    #region Server

    public static string DesktopPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    }

    public static void ServerDateTime(MonoBehaviour behaviour, Action<bool, DateTime> onEnd)
    {
        behaviour.StartCoroutine(ServerDateTimeCo((success, dateTime) => onEnd?.Invoke(success, dateTime)));
    }

    static IEnumerator ServerDateTimeCo(Action<bool, DateTime> onEnd)
    {
        var request = UnityWebRequest.Get("www.google.co.kr");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onEnd?.Invoke(false, DateTime.Now);
            yield break;
        }
        onEnd?.Invoke(true, Convert.ToDateTime(request.GetResponseHeader("Date")));
    }

    /// <summary> check internet connection </summary>
    public static bool CheckInternetConnect()
    {
        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        return InternetGetConnectedState(out int desc, 0);
    }

    #endregion
}
