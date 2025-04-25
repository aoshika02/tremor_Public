public enum EnemyState
{
    None                        = 0,
    Init                        = 1, //初期化
    Stay                        = 2, //待機
    Rotation                    = 3, //回転
    Move                        = 4, //移動
    FindPlayer                  = 5, //プレイヤーを捜索
    Dead                        = 6, //死亡
    DiscoveryPlayer             = 7, //プレイヤーを発見
    MovePlayer                  = 8, //プレイヤーを追いかける
    LostPlayer                  = 9, //プレイヤーを見失う
    Attack                      = 10, //プレイヤーを攻撃
    ReturnPosition              = 11, //プレイヤー発見前の位置に戻る
    ChaseDiscoveryPlayer        = 12, //チェイス時にプレイヤーを発見する専用のステート
    FlowBreak                   = 99  //実行フローを終了
}