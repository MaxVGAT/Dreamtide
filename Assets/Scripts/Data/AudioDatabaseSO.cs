using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Database")]
public class AudioDatabaseSO : ScriptableObject
{
    public List<AudioClipData> player;       // プレイヤー関連の音
    public List<AudioClipData> uiAudio;      // UI関連の音
    public List<AudioClipData> enemies;      // 敵関連の音
    public List<AudioClipData> npcs;         // NPC関連の音
    public List<AudioClipData> objects;      // オブジェクト関連の音

    [Header("Music List")]
    public List<AudioClipData> mainMenuMusic; // メインメニュー用BGM
    public List<AudioClipData> levelMusic;    // レベル用BGM

    private Dictionary<string, AudioClipData> clipCollection; // 名前で音を取得する辞書

    private void OnEnable()
    {
        clipCollection = new Dictionary<string, AudioClipData>();

        // 各リストを辞書に登録
        AddToCollection(player);
        AddToCollection(uiAudio);
        AddToCollection(enemies);
        AddToCollection(npcs);
        AddToCollection(objects);
        AddToCollection(mainMenuMusic);
        AddToCollection(levelMusic);
    }

    public AudioClipData Get(string groupName)
    {
        // 名前でAudioClipDataを取得、存在しなければnull
        return clipCollection.TryGetValue(groupName, out var data) ? data : null;
    }

    private void AddToCollection(List<AudioClipData> listToAdd)
    {
        foreach (var data in listToAdd)
        {
            if (data != null && clipCollection.ContainsKey(data.audioName) == false)
            {
                clipCollection.Add(data.audioName, data); // 辞書に登録
            }
        }
    }
}

[System.Serializable]
public class AudioClipData
{
    public string audioName;           // 音の識別名
    public List<AudioClip> clips = new List<AudioClip>(); // クリップのリスト
    [Range(0f, 1f)] public float maxVolume = 1f; // 最大音量

    public AudioClip GetRandomClip()
    {
        // クリップが存在する場合、ランダムに1つ返す
        if (clips == null || clips.Count == 0)
            return null;

        return clips[Random.Range(0, clips.Count)];
    }
}
