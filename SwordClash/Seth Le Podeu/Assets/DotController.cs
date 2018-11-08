using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordClash
{
    public class DotController : MonoBehaviour
    {

        public SpriteRenderer m_SpriteRenderer;

        public enum swipeEvent { UpSwipe, DownSwipe, LeftSwipe, RightSwipe };

        // Use this for initialization
        void Start()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnSwipe(swipeEvent whichSwipe)
        {
            switch (whichSwipe)
            {
                case swipeEvent.UpSwipe:
                    {
                        OnUpSwiped();
                        break;
                    }
                case swipeEvent.DownSwipe:
                    {
                        OnDownSwiped();
                        break;
                    }
                case swipeEvent.LeftSwipe:
                    {
                        OnLeftSwiped();
                        break;
                    }
                case swipeEvent.RightSwipe:
                    {
                        OnRightSwiped();
                        break;
                    }
                default:
                    //Console.WriteLine("Default case");
                    break;
            }
        }

        public void OnUpSwiped()
        {
            m_SpriteRenderer.color = Color.blue;
        }
        public void OnDownSwiped()
        {
            m_SpriteRenderer.color = Color.magenta;
        }
        public void OnLeftSwiped()
        {
            m_SpriteRenderer.color = Color.black;
        }
        public void OnRightSwiped()
        {
            m_SpriteRenderer.color = Color.green;
        }


    }
}