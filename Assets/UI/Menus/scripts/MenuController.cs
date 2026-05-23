using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private bool menuAberto = false;
    private string nomeDaCenaMenu = "MenuGestaoScene"; // Garanta que o nome está igual ao da cena

    void Update()
    {
        // Abre/Fecha o menu ao carregar na tecla M (pode mudar para o que quiser)
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!menuAberto)
            {
                AbrirMenuGestao();
            }
            else
            {
                FecharMenuGestao();
            }
        }
    }

    public void AbrirMenuGestao()
    {
        menuAberto = true;
        // O segredo está no LoadSceneMode.Additive!
        SceneManager.LoadScene(nomeDaCenaMenu, LoadSceneMode.Additive);

        // Se quiser libertar o rato para o jogador clicar nos botões do menu:
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void FecharMenuGestao()
    {
        menuAberto = false;
        // Descarrega apenas a cena do menu, mantendo o restaurante intacto
        SceneManager.UnloadSceneAsync(nomeDaCenaMenu);

        // Se quiser bloquear o rato de volta para o jogo 3D:
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}