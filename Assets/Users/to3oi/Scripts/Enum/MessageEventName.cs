public enum MessageEventName
{
    Test = 0,
    FootStepSound = 1,      // チュートリアルの足音確認
    LockedGate = 2,        // ゲートが開かないときのテキスト
    FrontLatch = 3,         // チュートリアル後 ノコギリのドアを調べたとき
    QuizEnterRoom = 4,     // クイズ
    //Quiz_TakeRadio = 5,   // 廃止
    GhostShow = 6,
    //GhostShow2 = 7,       // 廃止
    QuizStart = 8,                      // クイズ
    CryGirlBefore = 9,
    CryGirlAfter = 10,
    TransformGirl = 11,
    TransformedGirl = 12,
    QuizClear = 13,
    QuizHeart = 14,
    QuizSkull = 15,
    QuizMiss1 = 16,
    QuizMiss2 = 17,
    //RedPoint = 18,        // 廃止
    OpenBox = 19,
    ReTakeRadio = 20,
    PreparationRoom_NotPeek = 21,
    PreparationRoom_NotGetGey = 22,
    MusicRoom_GetKey = 23,      // 音楽室で鍵入手時のテキスト
    LookedPeepHole = 24,            // 準備室の穴を調べたときのテキスト
    NotGetCassette = 25,        // カセットテープ未取得時のテキスト
    RadioChecked = 26,          // ラジオを2つチェックしたあとのテキスト
    SawSynthesis = 27,          // ノコギリ合成語のテキスト
    GateOpened = 28,            // 中間ドア解放後のテキスト

    TakeRadio_1_1 = 100,
    TakeRadio_2_1 = 101,
    TakeRadio_3_1 = 102,
    TakeRadio_1_2 = 103,
    TakeRadio_2_2 = 104,
    TakeRadio_3_2 = 105,
}