using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropALV : MonoBehaviour
{
	private float liftedHeight; // altura que tendrá la bola después de elevarse
	private Plane dragPlane;
	[SerializeField] MoleculaObject molObj;

	//Al hacer click al objeto
	void OnMouseDown()
	{
		//plano horizontal donde arrastrar
		dragPlane = new Plane(Vector3.up, Vector3.zero);

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float enter;

		if (dragPlane.Raycast(ray, out enter))
		{
			Vector3 hitPoint = ray.GetPoint(enter);

			Vector3 dir = (transform.position - hitPoint).normalized;

			Vector3 liftedPos = transform.position + dir * 1;

			liftedHeight = liftedPos.y;

			transform.position = liftedPos;
		}
	}

	//cuando arrastras el mouse
	void OnMouseDrag()
	{
		dragPlane = new Plane(Vector3.up, new Vector3(0, liftedHeight, 0));

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float enter;

		if (dragPlane.Raycast(ray, out enter))
		{
			Vector3 hitPoint = ray.GetPoint(enter);
			transform.position = new Vector3(hitPoint.x, liftedHeight, hitPoint.z);
		}
	}

	void OnMouseUp()
	{
		MoleculaObject mol = GetComponent<MoleculaObject>();
		//creamos un colider imaginario alrededor de la esfera para saber si estamos en un alveolo o en una vena
		Collider[] cols = Physics.OverlapSphere(transform.position, 1f);

		bool foundZone = false;

		foreach (var h in cols)
		{
			TipeZone zone = h.GetComponent<TipeZone>();
			if (zone != null)
			{
				mol.ChangeTipe(zone.tipe);
				foundZone = true;
				break;
			}

		}

		if (!foundZone)
		{
			mol.SetNone();
		}
	}
}
