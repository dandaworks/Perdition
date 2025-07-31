using UnityEngine;
using piqey.Utilities.Editor;

public class PortalPulse : MonoBehaviour
{
	[SerializeField]
	private Transform Portal;

	[Range(0.0f, 360.0f)]
	public float Rate = 2.5f;
	// public Vector2 Offset = Vector2.zero;

	// public bool OffsetByHashCode = true;
	// [Range(0.0f, 128.0f)]
	// public float OffsetStartDistanceMax = 32.0f;

	[SerializeField, ReadOnly] private Vector3 _pivot;
	[SerializeField, ReadOnly] private Vector3 _axis;
	// [SerializeField, ReadOnly] private float _angle = 0.0f;
	// [SerializeField, ReadOnly] private float _baseOffset = 0.0f;

	private void Awake()
	{
		if (!Portal)
			Portal = transform;
	}

	private void Start()
	{
		// if (OffsetByHashCode)
			// _baseOffset = Random.value * OffsetStartDistanceMax;

		if (Portal.TryGetComponent(out Renderer renderer))
			_pivot = renderer.bounds.center;
		else
		{
			Debug.LogError($"{nameof(Portal)} has no {nameof(Renderer)}!");
			enabled = false;

			return;
		}

		_axis = Portal.up;
		// _angle = _baseOffset;
	}

	private void Update()
	{
		// _angle += Rate * Time.deltaTime;
		Portal.RotateAround(_pivot, _axis, Rate * Time.deltaTime);
	}
}
