namespace ArionDigital
{
    using UnityEngine;

    public class CrashCrate : MonoBehaviour
    {
        public MeshRenderer wholeCrate;
        public BoxCollider boxCollider;
        public GameObject fracturedCrate;
        public AudioSource crashAudioClip;

        private void OnTriggerEnter(Collider other)
        {
            wholeCrate.enabled = false;
            boxCollider.enabled = false;
            fracturedCrate.SetActive(true);
            crashAudioClip.Play();
        }

        [ContextMenu("Test")]
        public void Test()
        {
            wholeCrate.enabled = false;
            boxCollider.enabled = false;
            fracturedCrate.SetActive(true);
        }
    }
}