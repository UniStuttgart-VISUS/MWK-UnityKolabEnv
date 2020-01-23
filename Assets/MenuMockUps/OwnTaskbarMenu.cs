﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnTaskbarMenu : MonoBehaviour
{
    private OwnTaskbarMenuItem[] items;
    private int itemCounter;
    private const int lowestY = -65;
    // Start is called before the first frame update
    void Start()
    {
        itemCounter = 0;
        items = GetComponentsInChildren<OwnTaskbarMenuItem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void add(OwnTaskbarMenuItem item)
    {
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.AddComponent(new TaskbarMenuItem());


        int index = -1;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].name.Equals(item.name))
            {
                index = i;
                break;
            }
        }

        if (index > -1 && !item.isActiveAndEnabled) // the given item exists
        {
            items[index].gameObject.transform.position = new Vector3(0, lowestY + itemCounter * 50, 0);
            items[index].gameObject.SetActive(true);
        }

        itemCounter++;
        //if (itemCounter <= 5) //there are only 5 items yet
        //{
        //    items[itemCounter].gameObject.SetActive(true);
        //}
    }

    public void startInteraction(CheckboxScript Interaction, OwnTaskbarMenuItem item)
    {
        Interaction.gameObject.SetActive(true);

        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i].name.Equals(item.name))
            {
                items[i].Interaction.gameObject.SetActive(false);
            }
        }
    }

    public void stopInteraction(CheckboxScript Interaction, OwnTaskbarMenuItem item)
    {
        Interaction.gameObject.SetActive(false);
    }
}
