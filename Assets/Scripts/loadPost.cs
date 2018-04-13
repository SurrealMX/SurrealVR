using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class loadPost{

    private Dictionary<string, object> Feed_ref;

    loadPost(int postIndex)
    {
        FirebaseFeed aFeed = GameObject.FindGameObjectWithTag("PublicFeed").GetComponent<FirebaseFeed>();
        Feed_ref = aFeed.PublicFeed;

        if (postIndex < Feed_ref.Count)
        {
            object currentPost = Feed_ref.ElementAt(postIndex).Value;
            currentPost.
        }
        else
            return;
    }

}
