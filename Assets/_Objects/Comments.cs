using UnityEngine;
using System.Collections;

public class Comments : MonoBehaviour {

    [TextArea(8, 100)]
    public string comments;
    public TextAreaAttribute text;
}