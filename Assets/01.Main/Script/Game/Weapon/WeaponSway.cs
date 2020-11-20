using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    #region Field
    public float swayAmount = 0.25f;
	public float smoothAmount = 6f;
	public float maxAmount = 0.06f;

	private Vector3 originalPosition;
    #endregion

    #region Unity Methods
    void Start()
	{
		originalPosition = transform.localPosition;
	}

	void Update()
	{
		float positionX = -Input.GetAxis("Mouse X") * swayAmount;
		float positionY = -Input.GetAxis("Mouse Y") * swayAmount;

		Mathf.Clamp(positionX, -maxAmount, maxAmount);
		Mathf.Clamp(positionY, -maxAmount, maxAmount);

		Vector3 swayPosition = new Vector3(positionX, positionY, 0);
		transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition + swayPosition, Time.deltaTime * smoothAmount);
	}
    #endregion
}
