# どうぶつ推理オンライン
![対戦](https://user-images.githubusercontent.com/67674781/221357940-1524de6f-0771-41b2-8990-3a4039c13858.gif)

## 作品概要
### ジャンル
対戦型パズルゲーム

### ルール
2人のプレイヤーが交互に推理し、先にすべてのどうぶつと場所を当てたほうが勝ちとなります。

### 面白いポイント
自分の回答が相手のヒントにもなるため、勝つためには答えを推理するだけではなく、どのようにして相手から答えになる情報を引き出すかという駆け引きが魅力です。

## コンセプト
このゲームのコンセプトは「老若男女問わず一緒に遊べる脳トレゲーム」です。

そのうえで、「多くの人に考えることの楽しさ」を知ってもらうことを目標に大きく3つの軸を考えました。

|  軸  |  具体的には  | ねらい |
| ---- | ---- | ---- |
| 親しみやすさ | どの世代から見ても分かりやすく、親しみやすいUIとデザインにする | 年齢問わず遊ばれるようにすること |
| 気軽さ | 初心者でも上級者と気軽に遊べるようなゲームシステムにする | 多くの人に遊んでもらうこと|
| 上達の実感 | ゲームが上手くなっていることを実感できるような機能を追加する | 考えることの楽しさを知ってもらうこと |

この3つの「親しみやすさ、気軽さ、上達の実感」を軸に開発を進めていきました。

## プレイ時に見てほしい点
### 1.親しみやすさ
#### デザイン
- 全体的に淡白すぎないかつ派手すぎないようなデザインを目指しました。
- メインキャラクターとして動物を採用しました。
- 難しい漢字を減らしました。
- フォントに[NotoSans](https://fonts.google.com/noto/specimen/Noto+Sans+JP)を使いました。

#### UI・アニメーション
- ボタンを押した時にアニメーションやサウンドを入れました。
- 画面の遷移にアニメーションを入れました。
- アイコンをたくさん使いました。

### 2.気軽にプレイできる点
- 遊び方の説明に例題を作り、その場で試せるようにしました。
- オンライン対戦機能を追加し、どこでも対戦ができるようにしました。
- トレーニングモードを追加し、一人でコツコツ遊べるようにしました。

### 3.上達の実感ができる点
- プロフィールや対戦時に勝利数を表示するようにしました。
- ランキングを実装して世界中の人と勝利数を競い合うことができるようにしました。
- 自分の勝利数によってアイコンの縁が変化するようにしました。

## よくできたと思うポイント
1\. アニメーション  
 アニメーションの実装に[DOTWeen](http://dotween.demigiant.com/)というアセットを使用し、自由度の高いアニメーションを作ることができました。コードの管理は主に[DotweenAnimation.cs](https://github.com/t4ichi/AnimalSearchOnline/blob/master/Assets/Scripts/Animations/DotweenAnimations.cs)で、アニメーション拡張が自在にできるようにしました。

2\. オンライン対戦機能  
 オンライン対戦機能を作る上で、特に非同期処理が大変でした。非同期処理をするときには[コルーチン](https://docs.unity3d.com/ja/2018.4/Manual/Coroutines.html)を使って実装を進めました。また、ゲームプレイ中の処理は[PunTurnManager.cs](https://github.com/t4ichi/AnimalSearchOnline/blob/master/Assets/Scripts/Managers/PunTurnManager.cs)にあるようにコールバックを使用して、同期処理の回数を削減することができました。

3\. ランキング機能  
 世界中の人の勝利数をランキングにする機能を[NCMB](https://mbaas.nifcloud.com/)を使って実装しました。主に[LeaderBoardManager.cs](https://github.com/t4ichi/AnimalSearchOnline/blob/master/Assets/Scripts/LeaderBoard/LeaderBoardManager.cs)でデータの取得やアップロードの機能をまとめて管理しています。

4\. メニュー管理  
メニューの管理に[MenuManager](https://github.com/t4ichi/AnimalSearchOnline/blob/master/Assets/Scripts/Utility/_UI/_Menu/MenuManager.cs)のようにステートパターンを使いました。例えばゲーム画面からメイン画面へ戻りたい時には
```cs
MenuManager.Instance.SwitchMenu(MenuType.Main);
```
のように実行することで画面に遷移が簡単にできるようになります。

5\. データ管理  
データの管理には[スクリプタブルオブジェクト](https://docs.unity3d.com/ja/2018.4/Manual/class-ScriptableObject.html)を使用しました。例えば、動物のデータを追加するときには以下のようにエディター上で管理できるようにしました。
![add_character](https://user-images.githubusercontent.com/67674781/221606352-e21d2006-b437-4251-a28d-d76b6f4e6fa9.gif)

6\. 共有機能  
ゲームが終了した後にスコアをシェアする機能を[NatShare](https://github.com/natmlx/NatShare)というアセットを使って実装しました。また、ゲームスコアを画面キャプチャし、画像を取得する機能を[ShareManager.cs](https://github.com/t4ichi/AnimalSearchOnline/blob/master/Assets/Scripts/Utility/ShareManager.cs)で
実装し、簡単にシェアできるようにしました。

![share](https://user-images.githubusercontent.com/67674781/221609047-69f83883-1d77-44be-b032-cbe691d5c8ad.gif)


7\. NGワード  
オンラインで快適に遊んでもらうためにプレイヤーの名前に不快な言葉が使われないようにしました。[StringExtensions](https://github.com/t4ichi/AnimalSearchOnline/blob/master/Assets/Scripts/Utility/_Extensions/StringExtensions.cs)で拡張メソッドを用意し、[名前を入力した時](https://github.com/t4ichi/AnimalSearchOnline/blob/master/Assets/Scripts/Menu/Menus/MainMenu.cs#L261)に変更するかを判定しています。

## 制作概要
| 制作環境 | 内容 |
| ---- | ---- |
| ゲームエンジン | Unity |
| 制作人数 | 1人 |
| 制作期間 | 約3ヶ月 |
| 使用言語 | C# |
| 使用PC | MacBookAir |

| 使用アセット | 用途 |
| ---- | ---- |
| [DOTween](http://dotween.demigiant.com/) | アニメーションの実装 |
| [GoogleMobileAds](https://developers.google.com/admob) | 広告の実装 |
| [NatShare](https://github.com/natmlx/NatShare) | シェア機能の実装 |
| [NCMB](https://mbaas.nifcloud.com/) | リーダーボードの実装 |
| [Photon](https://www.photonengine.com/ja/) | オンライン対戦機能の実装 |

| 使用素材 | 用途 |
| ---- | ---- |
| [OtoLogic](https://otologic.jp/) | BGM・SE |
| [DOVA-SYNDROME](https://dova-s.jp/) | BGM・SE |
| [甘茶の音楽工房](https://amachamusic.chagasi.com/) | BGM・SE |
| [GoogleFonts](https://fonts.google.com/) | アイコン・イラスト |
| [flaticon](https://www.flaticon.com/) | アイコン・イラスト |
| [illustAC](https://www.ac-illust.com/) | キャラクターイラスト |
