using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoSceneToggle : MonoBehaviour {

	public List<GameObject> objects;
	public Text title;

	public float rotationSpeed = 90;

	private int index = 0;

	private void Start() {
		UpdateSelection();
	}

	private void Update() {
		foreach (GameObject obj in objects) {
			obj.transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
		}
	}

	public void Next() {
		index++;
		if (index >= objects.Count) index = 0;

		UpdateSelection();
	}

	private void UpdateSelection() {
		for (int i = 0; i < objects.Count; i++) {
			if (i == index) objects[i].SetActive(true);
			else objects[i].SetActive(false);
		}
		title.text = objects[index].name;
	}
}
