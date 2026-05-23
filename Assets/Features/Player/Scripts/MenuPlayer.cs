using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayer : MonoBehaviour
{
    [Header("Configurações do Menu")]
    [SerializeField] private string nomeDaCenaMenu = "MenuGestaoScene"; // Garante que o nome está igual ao da cena
    [SerializeField] private KeyCode teclaMenu = KeyCode.Q;

    private bool menuAberto = false;
    private bool aProcessar = false; // Evita bugs se o jogador carregar no Q repetidamente

    void Update()
    {
        // Deteta o clique na tecla "Q"
        if (Input.GetKeyDown(teclaMenu) && !aProcessar)
        {
            if (!menuAberto)
            {
                StartCoroutine(AbrirMenu());
            }
            else
            {
                StartCoroutine(FecharMenu());
            }
        }
    }

    private System.Collections.IEnumerator AbrirMenu()
    {
        aProcessar = true;
        menuAberto = true;

        // Carrega a cena do menu por cima (Additive)
        AsyncOperation op = SceneManager.LoadSceneAsync(nomeDaCenaMenu, LoadSceneMode.Additive);
        yield return op; // Espera a cena carregar completamente

        // Liberta o rato para o jogador conseguir interagir com o Painel de Gestão
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        aProcessar = false;
    }

    private System.Collections.IEnumerator FecharMenu()
    {
        aProcessar = true;
        menuAberto = false;

        // Descarrega apenas a cena do menu, mantendo o jogo a correr atrás
        AsyncOperation op = SceneManager.UnloadSceneAsync(nomeDaCenaMenu);
        yield return op; // Espera a cena fechar completamente

        // Volta a prender o rato no centro do ecrã para o jogo 3D (se o teu jogo for em 1ª ou 3ª pessoa)
        // Nota: Se o teu jogo principal for controlado apenas com cliques de rato, muda para CursorLockMode.None
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        aProcessar = false;
    }
}