using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class DragWeapon : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
	public static DragWeapon dragObject;
	public Transform parentTransform;
	private Vector3 tapRefPosition;
	private CanvasGroup canvasGroup;
	private Transform canvasTransform;

	public CanvasGroup CanvasGroup { get { return canvasGroup ?? (canvasGroup = gameObject.AddComponent<CanvasGroup>()); } }
	public Transform CanvasTransform { get { return canvasTransform ?? (canvasTransform = GameObject.Find("Canvas").transform); } }
	public Transform weapon;

	public void OnBeginDrag(PointerEventData eventData)
	{
		tapRefPosition = (Vector2)transform.position - eventData.position;
		dragObject = this;
		parentTransform = transform.parent;
		transform.SetParent(CanvasTransform);
		CanvasGroup.blocksRaycasts = false;
		CanvasGroup.alpha = 0.5f;
	}

	public void OnDrag(PointerEventData eventData)
	{
        transform.position = Input.mousePosition + tapRefPosition;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (transform.parent == canvasTransform)
		{
			transform.SetParent(parentTransform);
		}
		dragObject = null;
		CanvasGroup.blocksRaycasts = true;
		CanvasGroup.alpha = 1.0f;
		TPSShotController.Aceess(0).SelectNewWeapon(weapon);

	}

	public void Update()
	{
		if (dragObject == null)
		{
			transform.localPosition -= transform.localPosition / 3.0f;
		}
	}
}
