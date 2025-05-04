using System.Diagnostics;
using System.Media;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace MyConsoleApp;

public class Program
{
    // Game timers
    private static Stopwatch? _gameTimeStopwatch;
    private static Stopwatch _stopwatch = null!;
    private static Stopwatch? _stopwatchShoot;

    // Game stats
    private static float _fps;

    private static readonly int ScreenWidth = 120;
    private static readonly int ScreenHeight = 40;

    private static int _consoleWidth = Console.WindowWidth;
    private static int _consoleHeight = Console.WindowHeight;

    private static bool _mapVisible = true;
    private static bool _statsVisible = true;

    private static readonly TimeSpan GameTime = TimeSpan.FromSeconds(60);

    private static readonly int screenWidth = 120;
    private static readonly int screenHeight = 40;

    private static bool _gameOver;

    private static readonly float Depth = 16.0f;
    private static readonly float[,] DepthBuffer = new float[ScreenWidth, ScreenHeight];

    private static readonly char[,] Screen = new char[ScreenWidth, ScreenHeight];

    private static readonly List<(float X, float Y)> Boxes = [(13.5f, 09.5f)];

    // Player stats
    private static float _playerA;
    private static float _playerX;
    private static float _playerY;

    private static int _score;

    private static readonly float Fov = 3.14159f / 4.0f;

    private static Item _equippedItem = Item.Hand;

    private static readonly float Speed = 5.0f;
    private static readonly float RotationSpeed = 0.28f;

    private static int _delayedSeconds;
    private static int _equipedBoxesCount;

    // Animation stats
    private static readonly TimeSpan HandUseAnimationTime = TimeSpan.FromSeconds(0.2f);
    private static readonly TimeSpan BoxUseAnimationTime = TimeSpan.FromSeconds(0.5f);

    // Game handlers
    private static bool _backToMenu;
    private static bool _closeRequested;
    private static bool _screenLargeEnough = true;
    private static bool _isWalking;
    private static bool _isWalkPlayed;

    private static bool _isWindows = OperatingSystem.IsWindows();

    // Sound players
    private static SoundPlayer? _walkPlayer;
    private static SoundPlayer? _itemUsePlayer;
    private static SoundPlayer? _boxDeliveredPlayer;
    private static SoundPlayer? _menuPlayer;

    private static readonly string[] Map =
    [
        // (0,0)              (+,0)
        "███████████████████████████",
        "█           █$████        █",
        "█     █         █         █",
        "█     █                ██ █",
        "█ █████    █              █",
        "█                         █",
        "█                 ███     █",
        "█    ██                   █",
        "█           ███           █",
        "█                         █",
        "█                    ██████",
        "█    ███     ^            █",
        "█                         █",
        "███████████████████████████",
        // (0,+)              (+,+)
    ];

    private static readonly string[] BoxSprite1 =
    [
        "!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!",
        "╔═════════════╗",
        "║  ╭────╮     ║",
        "║  │╭─╮ │  ╭╮ ║",
        "║  ││ │ ╰──╰│ ║",
        "║  ││ ╰─────╯ ║",
        "╚═════════════╝",
    ];

    private static readonly string[] BoxSprite2 =
    [
        "!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!",
        "╔═══════════════╗",
        "║  ╭──────╮  *  ║",
        "║  │ ╭─╮  │     ║",
        "║  │ │ │  │ ╭─╮ ║",
        "║  │ │ │  ╰─╰ │ ║",
        "║  │ │ ╰──────╯ ║",
        "╚═══════════════╝",
    ];

    private static readonly string[] BoxSprite3 =
    [
        "!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!",
        "╔═══════════════════╗",
        "║  ╭─────────╮   *  ║",
        "║  │         │   *  ║",
        "║  │  ╭──╮   │      ║",
        "║  │  │  │   │      ║",
        "║  │  │  │   │ ╭──╮ ║",
        "║  │  │  │   ╰─╰  │ ║",
        "║  │  │  ╰────────╯ ║",
        "╚═══════════════════╝",
    ];

    private static readonly string[] BoxSprite4 =
    [
        "!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!",
        "╔══════════════════════╗",
        "║  ╭──────────╮   *    ║",
        "║  │          │  ***   ║",
        "║  │  ╭──╮    │   *    ║",
        "║  │  │  │    │        ║",
        "║  │  │  │    │  ╭───╮ ║",
        "║  │  │  │    │  │   │ ║",
        "║  │  │  │    ╰──╰   │ ║",
        "║  │  │  ╰───────────╯ ║",
        "╚══════════════════════╝",
    ];

    private static readonly string[] BoxSprite5 =
    [
        "!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "╔════════════════════════╗",
        @"║  ╭───────────╮  /*\    ║",
        "║  │           │ |***|   ║",
        @"║  │  ╭───╮    │  \*/    ║",
        "║  │  │   │    │         ║",
        "║  │  │   │    │  ╭────╮ ║",
        "║  │  │   │    │  │    │ ║",
        "║  │  │   │    │  │    │ ║",
        "║  │  │   │    ╰──╰    │ ║",
        "║  │  │   ╰────────────╯ ║",
        "╚════════════════════════╝",
    ];

    private static readonly string[] BoxSprite6 =
    [
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "╔═══════════════════════════╗",
        @"║ ╭─────────────╮    /*\    ║",
        "║ │             │  -|***|-  ║",
        @"║ │  ╭────╮     │    \*/    ║",
        "║ │__│    │     │           ║",
        "║ │  │    │     │   ╭─────╮ ║",
        "║ │  │    │     │   │     │ ║",
        "║ │  │    │     │   │     │ ║",
        "║ │  │    │     │   │     │ ║",
        "║ │  │    │     ╰───╰     │ ║",
        "║ │  │    ╰───────────────╯ ║",
        "╚═══════════════════════════╝",
    ];

    private static readonly string[] BoxSprite7 =
    [
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "╔═══════════════════════════════╗",
        @"║ ╭───────────────╮     /*\     ║",
        "║ │               │   -|***|-   ║",
        @"║ │  ╭─────╮      │     \*/     ║",
        "║ │  │     │      │      |      ║",
        "║ │__│     │      │    ╭──────╮ ║",
        "║ │  │     │      │    ╰──╮   │ ║",
        "║ │  │     │      │       │   │ ║",
        "║ │  │     │      │    ╭──╯   │ ║",
        "║ │  │     │      │    │   ╭──╯ ║",
        "║ │  │     │      ╰────╰   │    ║",
        "║ │  │     ╰───────────────╯    ║",
        "╚═══════════════════════════════╝",
    ];

    private static readonly string[] BoxSprite8 =
    [
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
        "╔═══════════════════════════════════════╗",
        @"║ ╭──────────────────────╮       |      ║",
        @"║ │                      │      /*\     ║",
        @"║ │   ╭──────╮           │   _ |***| _  ║",
        @"║ │   │      │           │      \*/     ║",
        @"║ │   │      │           │       |      ║",
        "║ │___│      │           │              ║",
        "║ │   │      │           │    ╭───────╮ ║",
        "║ │   │      │           │    ╰──╮    │ ║",
        "║ │   │      │           │       │    │ ║",
        "║ │   │      │           │       │    │ ║",
        "║ │   │      │           │    ╭──╯    │ ║",
        "║ │   │      │           │    │    ╭──╯ ║",
        "║ │   │      │           ╰────╰    │    ║",
        "║ │   │      ╰─────────────────────╯    ║",
        "╚═══════════════════════════════════════╝",
    ];

    private static readonly string[] PlayerHand =
    [
        "!!!!!!!!!",
        "!!!!!!!!!",
        "!!!!!!!!!",
        "╭───╭───╮",
        "│ ╰───╯ │",
        "│    ───╯",
        "│    ───╯",
        "╰╮  ╭──╯!",
    ];

    private static readonly string[] PlayerHandUse =
    [
        "!!!!!!!!!",
        "!!!!╭─╮!!",
        "╭─╮!│ │╮!",
        "│ │!│ ││╮",
        "│ ╰─╯ │││",
        "│      ││",
        "│      │╯",
        "╰╮   ╭─╯!",
    ];

    private static readonly string[] Box =
    [
        "!!!!!!!!!!!!",
        "!!!!!!!!!!!!",
        "!!!!!!!!!!!!",
        "!!!!!!!!!!!!",
        "!╔═══════╗!!",
        "!║ ╭─╮╭╮ ║╮!",
        "!║ │ ╰─╯ ║│!",
        "!╚═══════╝│!",
        "!!│       │!",
        "!!╰─╮   ╭─╯!",
    ];

    private static readonly string[] BoxUse =
    [
        "!!!!!!!!!!!!",
        "!!!!!!!!!!!!",
        "!!!!!!!!!!!!",
        "!╔═══════╗!!",
        "!║ ╭─╮╭╮ ║!!",
        "!║ │ ╰─╯ ║╮!",
        "!╚═══════╝│!",
        "! │ │ │ │ │!",
        "!!│       │!",
        "!!╰─╮   ╭─╯!",
    ];

    private static void DrawPlayer()
    {
        for (var i = 0; i < Map.Length; i++)
        {
            for (var j = 0; j < Map[i].Length; j++)
            {
                if (Map[i][j] is '^' or '<' or '>' or 'v')
                {
                    _playerY = i + .5f;
                    _playerX = j + .5f;
                    _playerA = Map[i][j] switch
                    {
                        '^' => 4.71f,
                        '>' => 0.00f,
                        '<' => 3.14f,
                        'v' => 1.57f,
                        _ => throw new NotImplementedException(),
                    };
                }
            }
        }
    }

    public static void Main(string[] args)
    {
        if (_isWindows)
        {
            _walkPlayer = new SoundPlayer("8bit-walk.wav");

            _walkPlayer.Load();

            _boxDeliveredPlayer = new SoundPlayer("box-delivered.wav");

            _boxDeliveredPlayer.Load();

            _itemUsePlayer = new SoundPlayer("item-use.wav");

            _itemUsePlayer.Load();

            _menuPlayer = new SoundPlayer("menu-music.wav");

            _menuPlayer.Load();
        }

        PlayAgain:

        DrawPlayer();

        Console.OutputEncoding = Encoding.UTF8;
        Console.Clear();

        if (_isWindows)
            _menuPlayer?.PlayLooping();

        Console.WriteLine(
            """
            Приветствую вас.

            Вас наняли в 'Red flag co' рабочим для перетаскивания коробок.
            Это ваш первый день. Вы же не хотите опозориться?
            Вам нужно брать коробки и нести их со всех ног к приемному пункту ($ на карте).
            Время ограничено, но за каждую коробку оставшиеся время 
            будет увеличиваться.
            Удачи!

            Управление
            - W, A, S, D: ходьба/поворот камеры
            - Прбел: подобрать коробку
            - 1: рука
            - 2: коробка (если есть)
            - M: включить/отключить карту
            - Tab: включить/отключить статистику
            - Escape: выход

            Нажмите любую клавишу, чтобы начать...
            """
        );

        if (Console.ReadKey(true).Key is not ConsoleKey.Escape)
        {
            if (_isWindows)
                _menuPlayer?.Stop();

            _gameTimeStopwatch = Stopwatch.StartNew();

            Console.Clear();

            _stopwatch = Stopwatch.StartNew();

            while (!_closeRequested)
            {
                Update();
                if (_backToMenu)
                {
                    _backToMenu = false;
                    goto PlayAgain;
                }

                Render();
            }
        }

        Console.Clear();

        Console.Write("Игра была закрыта.");
    }

    private static void Update()
    {
        if (_gameTimeStopwatch.Elapsed - TimeSpan.FromSeconds(_delayedSeconds) > GameTime)
        {
            _gameOver = true;
            _gameTimeStopwatch.Stop();
        }

        var u = false;
        var d = false;
        var l = false;
        var r = false;

        while (Console.KeyAvailable)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Enter:
                    _backToMenu = true;
                    break;

                case ConsoleKey.Escape:
                    _closeRequested = true;
                    return;

                case ConsoleKey.M:
                    if (!_gameOver)
                    {
                        _mapVisible = !_mapVisible;
                    }

                    break;
                case ConsoleKey.Tab:
                    if (!_gameOver)
                    {
                        _statsVisible = !_statsVisible;
                    }

                    break;
                case ConsoleKey.D1
                or ConsoleKey.NumPad1:
                    if (!_gameOver && PlayerIsNotBusy())
                    {
                        _equippedItem = Item.Hand;
                    }

                    break;
                case ConsoleKey.D2
                or ConsoleKey.NumPad2:
                    if (!_gameOver && PlayerIsNotBusy() && _equipedBoxesCount > 0)
                    {
                        _equippedItem = Item.Box;
                    }

                    break;
                case ConsoleKey.Spacebar:
                    if (!_gameOver && PlayerIsNotBusy())
                    {
                        if (_isWindows)
                            _itemUsePlayer?.PlaySync();

                        List<(float X, float Y)> equipedBoxes = [];
                        var spawnBox = false;

                        foreach (var box in Boxes)
                        {
                            var angle = (float)Math.Atan2(box.Y - _playerY, box.X - _playerX);

                            if (angle < 0)
                                angle += 2f * (float)Math.PI;

                            var distance = Vector2.Distance(
                                new Vector2(_playerX, _playerY),
                                new Vector2(box.X, box.Y)
                            );

                            var fovAngleA = _playerA - Fov / 2;

                            if (fovAngleA < 0)
                                fovAngleA += 2 * (float)Math.PI;

                            var diff =
                                angle < fovAngleA && fovAngleA - 2f * (float)Math.PI + Fov > angle
                                    ? angle + 2f * (float)Math.PI - fovAngleA
                                    : angle - fovAngleA;

                            var ratio = diff / Fov;
                            var enemyScreenX = (int)(ScreenWidth * ratio);

                            var boxSprite = distance switch
                            {
                                <= 01f => BoxSprite8,
                                <= 02f => BoxSprite7,
                                <= 03f => BoxSprite6,
                                <= 04f => BoxSprite5,
                                <= 05f => BoxSprite4,
                                <= 06f => BoxSprite3,
                                <= 07f => BoxSprite2,
                                _ => BoxSprite1,
                            };

                            var halfBoxWidth = boxSprite[0].Length / 2;
                            var boxMinScreenX = enemyScreenX - halfBoxWidth;
                            var boxMaxScreenX = enemyScreenX + halfBoxWidth;
                            var screenWidthMid = ScreenWidth / 2;

                            switch (_equippedItem)
                            {
                                case Item.Hand:
                                    if (
                                        boxMinScreenX <= screenWidthMid
                                        && screenWidthMid <= boxMaxScreenX
                                        && distance <= 3
                                    )
                                    {
                                        _equipedBoxesCount++;
                                        equipedBoxes.Add(box);
                                        spawnBox = true;
                                    }

                                    break;
                                case Item.Box:
                                    if (
                                        boxMinScreenX <= screenWidthMid
                                        && screenWidthMid <= boxMaxScreenX
                                    )
                                    {
                                        //equipedBoxes.Add(enemy);
                                        //spawnBox = true;
                                    }

                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }

                        if (_playerX > 13 && _playerY < 2 && _equipedBoxesCount != 0)
                        {
                            if (_isWindows)
                                _boxDeliveredPlayer?.PlaySync();

                            _score += _equipedBoxesCount;

                            _equipedBoxesCount = 0;
                            _equippedItem = Item.Hand;

                            _delayedSeconds += 5;
                        }

                        foreach (var box in equipedBoxes)
                        {
                            Boxes.Remove(box);
                        }

                        if (spawnBox)
                        {
                            SpawnTarget();
                        }

                        _stopwatchShoot = Stopwatch.StartNew();
                    }

                    break;
                case ConsoleKey.W:
                    if (!_gameOver)
                    {
                        u = true;
                    }

                    break;
                case ConsoleKey.A:
                    if (!_gameOver)
                    {
                        l = true;
                    }

                    break;
                case ConsoleKey.S:
                    if (!_gameOver)
                    {
                        d = true;
                    }

                    break;
                case ConsoleKey.D:
                    if (!_gameOver)
                    {
                        r = true;
                    }

                    break;
            }
        }

        if (_consoleWidth != Console.WindowWidth || _consoleHeight != Console.WindowHeight)
        {
            Console.Clear();

            _consoleWidth = Console.WindowWidth;
            _consoleHeight = Console.WindowHeight;
        }

        _screenLargeEnough = _consoleWidth >= ScreenWidth && _consoleHeight >= ScreenHeight;

        if (!_screenLargeEnough)
        {
            return;
        }

        var elapsedSeconds = (float)_stopwatch.Elapsed.TotalSeconds;

        _fps = 1.0f / elapsedSeconds;
        _stopwatch.Restart();

        if (_isWindows)
        {
            u = u || User32_dll.GetAsyncKeyState('W') is not 0 && !_gameOver;
            l = l || User32_dll.GetAsyncKeyState('A') is not 0 && !_gameOver;
            d = d || User32_dll.GetAsyncKeyState('S') is not 0 && !_gameOver;
            r = r || User32_dll.GetAsyncKeyState('D') is not 0 && !_gameOver;
        }

        if (d || u && !_gameOver)
            _isWalking = true;
        else
            _isWalking = false;

        if (l && !r)
        {
            _playerA -= (Speed * RotationSpeed) * elapsedSeconds;

            if (_playerA < 0)
            {
                _playerA %= (float)Math.PI * 2;
                _playerA += (float)Math.PI * 2;
            }
        }

        if (r && !l)
        {
            _playerA += (Speed * RotationSpeed) * elapsedSeconds;

            if (_playerA > (float)Math.PI * 2)
            {
                _playerA %= (float)Math.PI * 2;
            }
        }

        if (u && !d)
        {
            _playerX += (float)Math.Cos(_playerA) * Speed * elapsedSeconds;
            _playerY += (float)Math.Sin(_playerA) * Speed * elapsedSeconds;

            if (Map[(int)_playerY][(int)_playerX] is '█')
            {
                _playerX -= (float)Math.Cos(_playerA) * Speed * elapsedSeconds;
                _playerY -= (float)Math.Sin(_playerA) * Speed * elapsedSeconds;
            }
        }

        if (d && !u)
        {
            _playerX -= (float)(Math.Cos(_playerA) * Speed * elapsedSeconds);
            _playerY -= (float)(Math.Sin(_playerA) * Speed * elapsedSeconds);

            if (Map[(int)_playerY][(int)_playerX] is '█')
            {
                _playerX += (float)Math.Cos(_playerA) * Speed * elapsedSeconds;
                _playerY += (float)Math.Sin(_playerA) * Speed * elapsedSeconds;
            }
        }

        if (_isWalking && _isWindows && !_isWalkPlayed)
        {
            _walkPlayer?.PlayLooping();
            _isWalkPlayed = true;
        }

        if (!_isWalking && _isWindows)
        {
            _walkPlayer?.Stop();
            _isWalkPlayed = false;
        }
    }

    private static void SpawnTarget()
    {
        List<(float X, float Y)> possibleSpawnPoints = [];

        for (var y = 0; y < Map.Length; y++)
        {
            for (var x = 0; x < Map[y].Length; x++)
            {
                if (Map[y][x] is ' ')
                {
                    possibleSpawnPoints.Add((x + .5f, y + .5f));
                }
            }
        }

        var location = possibleSpawnPoints[Random.Shared.Next(possibleSpawnPoints.Count)];

        Boxes.Add(location);
    }

    private static bool PlayerIsNotBusy() =>
        _stopwatchShoot is null
        || _stopwatchShoot.Elapsed
            > _equippedItem switch
            {
                Item.Hand => HandUseAnimationTime,
                Item.Box => BoxUseAnimationTime,
                _ => throw new NotImplementedException(),
            };

    private static void Render()
    {
        if (!_screenLargeEnough)
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);

            Console.WriteLine($"Увеличте размер окна консоли...");

            Console.WriteLine($"Текущий размер: {_consoleWidth}x{_consoleHeight}");
            Console.WriteLine($"Мнимальный размер: {screenWidth}x{screenHeight}");

            return;
        }

        for (var y = 0; y < screenHeight; y++)
        {
            for (var x = 0; x < screenWidth; x++)
            {
                DepthBuffer[x, y] = float.MaxValue;
            }
        }

        for (var x = 0; x < screenWidth; x++)
        {
            var rayAngle = (_playerA - Fov / 2.0f) + (x / (float)screenWidth) * Fov;

            var stepSize = 0.1f;
            var distanceToWall = 0.0f;

            var hitWall = false;
            var boundary = false;

            var eyeX = (float)Math.Cos(rayAngle);
            var eyeY = (float)Math.Sin(rayAngle);

            while (!hitWall && distanceToWall < Depth)
            {
                distanceToWall += stepSize;

                var testX = (int)(_playerX + eyeX * distanceToWall);
                var testY = (int)(_playerY + eyeY * distanceToWall);

                if (testY < 0 || testY >= Map.Length || testX < 0 || testX >= Map[testY].Length)
                {
                    hitWall = true;
                    distanceToWall = Depth;
                }
                else
                {
                    if (Map[testY][testX] == '█')
                    {
                        hitWall = true;
                        List<(float, float)> p = [];

                        for (var tx = 0; tx < 2; tx++)
                        {
                            for (var ty = 0; ty < 2; ty++)
                            {
                                var vy = (float)testY + ty - _playerY;
                                var vx = (float)testX + tx - _playerX;
                                var d = (float)Math.Sqrt(vx * vx + vy * vy);
                                var dot = (eyeX * vx / d) + (eyeY * vy / d);

                                p.Add((d, dot));
                            }
                        }

                        p.Sort((a, b) => a.Item1.CompareTo(b.Item1));

                        var fBound = 0.005f;

                        if (Math.Acos(p[0].Item2) < fBound)
                            boundary = true;
                        if (Math.Acos(p[1].Item2) < fBound)
                            boundary = true;
                        if (Math.Acos(p[2].Item2) < fBound)
                            boundary = true;
                    }
                }
            }

            var ceiling = (int)(screenHeight / 2.0f - screenHeight / distanceToWall);
            var floor = screenHeight - ceiling;

            for (var y = 0; y < screenHeight; y++)
            {
                DepthBuffer[x, y] = distanceToWall;

                if (y <= ceiling)
                {
                    Screen[x, y] = ' ';
                }
                else if (y > ceiling && y <= floor)
                {
                    Screen[x, y] =
                        boundary ? ' '
                        : distanceToWall < Depth / 3.00f ? '█'
                        : distanceToWall < Depth / 1.75f ? '■'
                        : distanceToWall < Depth / 1.00f ? '▪'
                        : ' ';
                }
                else
                {
                    var b = 1.0f - ((y - screenHeight / 2.0f) / (screenHeight / 2.0f));

                    Screen[x, y] = b switch
                    {
                        < 0.20f => '●',
                        < 0.40f => '•',
                        < 0.60f => '·',
                        _ => ' ',
                    };
                }
            }
        }

        var fovAngleA = _playerA - Fov / 2;

        if (fovAngleA < 0)
            fovAngleA += 2 * (float)Math.PI;

        foreach (var box in Boxes)
        {
            var angle = (float)Math.Atan2(box.Y - _playerY, box.X - _playerX);

            if (angle < 0)
                angle += 2f * (float)Math.PI;

            var distance = Vector2.Distance(
                new Vector2(_playerX, _playerY),
                new Vector2(box.X, box.Y)
            );

            var ceiling = (int)(screenHeight / 2.0f - screenHeight / distance);
            var floor = screenHeight - ceiling;

            var boxSprite = distance switch
            {
                <= 01f => BoxSprite8,
                <= 02f => BoxSprite7,
                <= 03f => BoxSprite6,
                <= 04f => BoxSprite5,
                <= 05f => BoxSprite4,
                <= 06f => BoxSprite3,
                <= 07f => BoxSprite2,
                _ => BoxSprite1,
            };

            var diff =
                angle < fovAngleA && fovAngleA - 2f * (float)Math.PI + Fov > angle
                    ? angle + 2f * (float)Math.PI - fovAngleA
                    : angle - fovAngleA;

            var ratio = diff / Fov;
            var enemyScreenX = (int)(screenWidth * ratio);

            var enemyScreenY = Math.Min(floor, Screen.GetLength(1));

            for (var y = 0; y < boxSprite.Length; y++)
            {
                for (var x = 0; x < boxSprite[y].Length; x++)
                {
                    if (boxSprite[y][x] is not '!')
                    {
                        var screenX = x - boxSprite[y].Length / 2 + enemyScreenX;
                        var screenY = y - boxSprite.Length + enemyScreenY;

                        if (
                            0 <= screenX
                            && screenX <= screenWidth - 1
                            && 0 <= screenY
                            && screenY <= screenHeight - 1
                            && DepthBuffer[screenX, screenY] > distance
                        )
                        {
                            Screen[screenX, screenY] = boxSprite[y][x];
                            DepthBuffer[screenX, screenY] = distance;
                        }
                    }
                }
            }
        }

        if (_statsVisible)
        {
            string[] stats =
            [
                $"fps={_fps:0.}",
                $"Коробки={_equipedBoxesCount}",
                $"очки={_score}",
                $"таймер={(int)_gameTimeStopwatch!.Elapsed.TotalSeconds - _delayedSeconds}/{(int)GameTime.TotalSeconds}",
            ];
            for (var i = 0; i < stats.Length; i++)
            {
                for (var j = 0; j < stats[i].Length; j++)
                {
                    Screen[screenWidth - stats[i].Length + j, i] = stats[i][j];
                }
            }
        }

        if (_mapVisible)
        {
            for (var y = 0; y < Map.Length; y++)
            {
                for (var x = 0; x < Map[y].Length; x++)
                {
                    Screen[x, y] = Map[y][x] is '^' or '<' or '>' or 'v' ? ' ' : Map[y][x];
                }
            }

            foreach (var enemy in Boxes)
            {
                Screen[(int)enemy.X, (int)enemy.Y] = 'X';
            }

            Screen[(int)_playerX, (int)_playerY] = _playerA switch
            {
                >= 0.785f and < 2.356f => 'v',
                >= 2.356f and < 3.927f => '<',
                >= 3.927f and < 5.498f => '^',
                _ => '>',
            };
        }

        var player =
            _equippedItem is Item.Hand
            && _stopwatchShoot is not null
            && _stopwatchShoot.Elapsed < HandUseAnimationTime
                ? PlayerHandUse
            : _equippedItem is Item.Box
            && _stopwatchShoot is not null
            && _stopwatchShoot.Elapsed < BoxUseAnimationTime
                ? BoxUse
            : _equippedItem is Item.Hand ? PlayerHand
            : _equippedItem is Item.Box ? Box
            : throw new NotImplementedException();

        for (var y = 0; y < player.Length; y++)
        {
            for (var x = 0; x < player[y].Length; x++)
            {
                if (player[y][x] is not '!')
                {
                    Screen[
                        x + screenWidth / 2 - player[y].Length / 2,
                        screenHeight - player.Length + y
                    ] = player[y][x];
                }
            }
        }

        if (_gameOver)
        {
            string[] gameOverMessage =
            [
                $"                                        ",
                $"               Игра окончена!               ",
                $"                Счет: {_score}                ",
                $"   Нажмите [enter] чтобы вернуться в меню...   ",
                $"                                        ",
            ];

            var gameOverMessageY = screenHeight / 2 - gameOverMessage.Length / 2;

            foreach (var line in gameOverMessage)
            {
                var gameOverMessageX = screenWidth / 2 - line.Length / 2;

                foreach (var c in line)
                {
                    Screen[gameOverMessageX, gameOverMessageY] = c;
                    gameOverMessageX++;
                }

                gameOverMessageY++;
            }
        }

        StringBuilder render = new();

        for (var y = 0; y < Screen.GetLength(1); y++)
        {
            for (var x = 0; x < Screen.GetLength(0); x++)
            {
                render.Append(Screen[x, y]);
            }

            if (y < Screen.GetLength(1) - 1)
            {
                render.AppendLine();
            }
        }

        Console.CursorVisible = false;
        Console.SetCursorPosition(0, 0);
        Console.Write(render);
    }
}
