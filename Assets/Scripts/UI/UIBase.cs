using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIBase : MonoBehaviour
{
    private Dictionary<string, GameObject> _objects = new Dictionary<string, GameObject>();
    private Dictionary<string, Component> _components = new Dictionary<string, Component>();

    private void Awake()
    {
        Bind();
    }

    private void Bind()
    {
        // �ڽ����� �ִ� ��� transform�� ���´�.
        Transform[] arrTransform = transform.GetComponentsInChildren<Transform>(true);

        // ��ȸ�ϸ� Bind
        for (int i = 0; i < arrTransform.Length; i++)
        {
            _objects.TryAdd(arrTransform[i].gameObject.name, arrTransform[i].gameObject);
        }
    }

    public GameObject Get(string name)
    {
        // name�� ���� GameObject�� �޾ƿ´�.
        if (_objects.TryGetValue(name, out GameObject go) == false)
            Debug.LogWarning($"Can't find \"{name}\"");

        // ��ȯ
        return go;
    }

    public T Get<T>(string name) where T : Component
    {
        // Ű ����
        string key = $"{name}_{typeof(T).Name}";
        if (_components.TryGetValue(key, out Component component))
        {
            return component as T;
        }

        // ��ųʸ��� ĳ�̵� ������Ʈ�� ������ GameObject ���� �޾ƿ´�.
        GameObject go = Get(name);

        // GameObject�� ��ȿ���� ������ null ��ȯ
        if (go == null)
            return null;

        // GameObject�� ���� GetComponent ����
        component = go.GetComponent<T>();

        // �ش� ������Ʈ�� ������ null ��ȯ
        if (component == null)
        {
            Debug.LogWarning($"Can't find Component \"{typeof(T).Name}\" in \"{name}\"");
            return null;
        }

        // ������Ʈ�� ã�Ҵٸ� ĳ��
        _components.Add(key, component);

        // ������Ʈ ��ȯ
        return component as T;
    }

    public void AddUIEvent(GameObject go, Enums.UIEvent eType, UnityAction<PointerEventData> action)
    {
        // �̺�Ʈ�� �߰��� ���ӿ�����Ʈ�κ��� UIEventHandler ������Ʈ�� �޾ƿ´�
        // ���� ������Ʈ�� ���ٸ� �߰��Ѵ�.
        UIEventHandler handler = go.GetOrAddComponent<UIEventHandler>();

        switch (eType)
        {
            case Enums.UIEvent.PointerClick:
                handler.OnPointerClicked += action;
                break;
        }
    }

    public void RemoveUIEvent(GameObject go, Enums.UIEvent eType, UnityAction<PointerEventData> action)
    {
        UIEventHandler handler = go.GetComponent<UIEventHandler>();
        if (handler == null)
            return;

        switch (eType)
        {
            case Enums.UIEvent.PointerClick:
                handler.OnPointerClicked -= action;
                break;
        }
    }
}
