using UnityEngine;

public interface OnTriggerEnterInterface
{
    public void CallOnTriggerEnter(Collider other,CallOnTrigger callOnTrigger) { }
}

public interface OnTriggerStayInterface
{
    public void CallOnTriggerStay(Collider other,CallOnTrigger callOnTrigger) { }
}

public interface OnTriggerExitInterface
{
    public void CallOnTriggerExit(Collider other,CallOnTrigger callOnTrigger) { }
}