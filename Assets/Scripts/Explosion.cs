/* Copyright (c) Vander Amaral
 * This code holds the Sprite Sheet Animation
 * I tried to make it the best and easy to change.
 * You can polish it even more, and add functions to it if you bought.
 */ 

using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	
	
	private int x = 0;
	private int y = 0;
	public float speed = 0.03f;
	private float nextFire = 0.0f;
	public bool end = false;
	public bool rotate;
	
	// Update is called once per frame
	void LateUpdate () {
		
		if(Time.time > nextFire && !end){
			if(y < 4){
			if(x < 5){
	      	 	GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.25f*x,0.25f*y);
			 	x++;
			}
			if(x == 4){
				y++;
				x=0;
			}
			nextFire = Time.time + speed;
		}
			if(x==0 && y==4){
				end = true;
				Destroy(gameObject);
			}
		}
		if(rotate){
			transform.Rotate(2,0,0);
		}
		
	}
	

	
}
