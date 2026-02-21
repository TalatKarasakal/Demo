using UnityEngine;
using System.Collections.Generic;

public class DilYoneticisi : MonoBehaviour
{
    public static DilYoneticisi Instance;

    public enum Diller { Ingilizce, Turkce }
    public Diller seciliDil = Diller.Ingilizce;

    public delegate void DilDegisimi();
    public static event DilDegisimi OnDilDegisti;

    private Dictionary<string, string> ingilizceSozluk;
    private Dictionary<string, string> turkceSozluk;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SozlukleriDoldur();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SozlukleriDoldur()
    {
        ingilizceSozluk = new Dictionary<string, string>();
        ingilizceSozluk.Add("gameOver", "Game Over");
        ingilizceSozluk.Add("paused", "Paused");
        ingilizceSozluk.Add("playing", "Playing");
        ingilizceSozluk.Add("ready", "Ready");
        ingilizceSozluk.Add("wins", "Wins!");
        ingilizceSozluk.Add("round", "Round");
        ingilizceSozluk.Add("normalPlay", "Normal Play");
        ingilizceSozluk.Add("arcadePlay", "Arcade Play");
        ingilizceSozluk.Add("quit", "Quit");
        ingilizceSozluk.Add("resume", "Resume");
        ingilizceSozluk.Add("restart", "Restart");
        ingilizceSozluk.Add("mainMenu", "Main Menu");
        ingilizceSozluk.Add("playAgain", "Play Again");

        ingilizceSozluk.Add("player", "Player");
        ingilizceSozluk.Add("ai", "AI");
        ingilizceSozluk.Add("easy", "EASY");
        ingilizceSozluk.Add("medium", "MEDIUM");
        ingilizceSozluk.Add("hard", "HARD");
        ingilizceSozluk.Add("chooseLevel", "-CHOOSE YOUR LEVEL-");
        ingilizceSozluk.Add("dashMode", "Dash Mode");
        ingilizceSozluk.Add("pausedTitle", "-PAUSED-");

        ingilizceSozluk.Add("theme", "Theme");
        ingilizceSozluk.Add("themePastel", "Pastel");
        ingilizceSozluk.Add("themeDark", "Dark");
        ingilizceSozluk.Add("themeClassic", "Classic");

        turkceSozluk = new Dictionary<string, string>();
        turkceSozluk.Add("gameOver", "Oyun Bitti");
        turkceSozluk.Add("paused", "Duraklatıldı");
        turkceSozluk.Add("playing", "Oynanıyor");
        turkceSozluk.Add("ready", "Hazır");
        turkceSozluk.Add("wins", "Kazandı!");
        turkceSozluk.Add("round", "Tur");
        turkceSozluk.Add("normalPlay", "Normal Oyun");
        turkceSozluk.Add("arcadePlay", "Arcade Oyun");
        turkceSozluk.Add("quit", "Çıkış");
        turkceSozluk.Add("resume", "Devam Et");
        turkceSozluk.Add("restart", "Yeniden Başlat");
        turkceSozluk.Add("mainMenu", "Ana Sayfa");
        turkceSozluk.Add("playAgain", "Tekrar Oyna");

        turkceSozluk.Add("player", "Oyuncu");
        turkceSozluk.Add("ai", "Yapay Zeka");
        turkceSozluk.Add("easy", "KOLAY");
        turkceSozluk.Add("medium", "ORTA");
        turkceSozluk.Add("hard", "ZOR");
        turkceSozluk.Add("chooseLevel", "-SEVİYENİ SEÇ-");
        turkceSozluk.Add("dashMode", "Atılma Modu");
        turkceSozluk.Add("pausedTitle", "-DURAKLATILDI-");

        turkceSozluk.Add("theme", "Tema");
        turkceSozluk.Add("themePastel", "Pastel");
        turkceSozluk.Add("themeDark", "Karanlik");
        turkceSozluk.Add("themeClassic", "Klasik");
    }

    public string CeviriAl(string anahtarKelimeler)
    {
        if (seciliDil == Diller.Turkce && turkceSozluk.ContainsKey(anahtarKelimeler))
            return turkceSozluk[anahtarKelimeler];
        else if (seciliDil == Diller.Ingilizce && ingilizceSozluk.ContainsKey(anahtarKelimeler))
            return ingilizceSozluk[anahtarKelimeler];

        return anahtarKelimeler;
    }

    public void DilDegistir(Diller yeniDil)
    {
        seciliDil = yeniDil;
        OnDilDegisti?.Invoke();
    }
}