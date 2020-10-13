using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slide : MonoBehaviour
{
    // Start is called before the first frame update
    public Button btnSlide;
    public Animator animator;
    public void klikButton()
    {
        animator.SetBool("UpStair", true);
    }
    void OnEnable()
    {
        //Register Button Events
        btnSlide.onClick.AddListener(() => klikButton());

    }


    void OnDisable()
    {
        //Un-Register Button Events
        btnSlide.onClick.RemoveAllListeners();
    }
}
