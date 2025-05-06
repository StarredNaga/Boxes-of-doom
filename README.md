# **Boxes of Doom**

![ezcv logo](https://github.com/StarredNaga/Boxes-of-doom/blob/master/GameScreenShot2.bmp)


![ezcv logo](https://github.com/StarredNaga/Boxes-of-doom/blob/master/GameScreenShot.bmp)

### Краткое описание

Проект, созданный `для хакатона на тему "склад"`. В нём я попытался повторить механику игры **DOOM** в консоли.

---

### О себе

Мне 16 лет. Считаю себя уровнем pre junior.
Уже некоторое время изучаю c#, нравится видеть результат.
Очень хочу победить, но даже если не выйграю это был хороший опыт.
Рад, что многим мой проект понравился (хотя и сделал не совсем то ( Ч - Ч )), это сильно поднимает дух и желание развиваться дальше.

---

### Что хотел реализовать

С самого начала я хотел сделать **интересную игру на время**.  
Изначально идея была ближе к **тетрису** — коробки падают сверху, а игрок должен не дать им упасть.  
Позже остановился на концепции **псевдо-3D**, так как уже имел с ней опыт.  
Хотел добавить **сюжет** и **локации**, но это оказалось слишком амбициозно для сроков хакатона.

---

### Реализация

Эффект глубины достигается путём **отрисовки символов в зависимости от расстояния** до объектов (стен и коробок).  
При взаимодействии с объектом происходит проверка **дистанции** и **угла поворота камеры**.

---

### Что получилось в итоге

На мой взгляд, вышла **неплохая псевдо-3D игра**, особенно с учётом ограниченного времени.

---

### Правила игры

- У вас есть **60 секунд**, чтобы доставить как можно больше коробок до целевой точки.
- За каждую доставленную коробку **время уменьшается**.
- Коробки появляются **в случайной точке** карты — удача тоже играет роль.

**Мой рекорд — 13 коробок. Жду, когда кто-то побьёт его!**

---

### Плюсы и минусы

#### Минусы:
- Игра работает корректно **только в консоли под Windows**.
- При воспроизведении звуков **игра замирает**.
- Управление **не самое удобное**.

#### Плюсы:
- **Необычная стилистика**
- **Захватывающий геймплей**

---

### Послесловие

Я доволен результатом — игра действительно увлекает, чувствуется **соревновательный дух**.  
Надеюсь, проект получит признание!

---

# **Boxes of Doom**

### Brief Description

This project was created for a `warehouse-themed hackathon`.  
I tried to replicate **DOOM-style mechanics** in a console environment.

---

### Initial Concept

From the beginning, I wanted to make a **fun, time-based game**.  
At first, the idea was something like **Tetris** — boxes would fall from above, and the player had to prevent them from stacking.  
Eventually, I switched to a **pseudo-3D** concept, as I had some previous experience with it.  
I had plans to add a **storyline** and **locations**, but it turned out to be a bit too ambitious.

---

### Implementation

Depending on the **distance to walls or boxes**, I render characters on the screen to simulate **depth and 3D**.  
When interacting with objects, the game checks both the **distance** and the **camera rotation angle**.

---

### Final Result

In my opinion, the result is a **decent pseudo-3D game**, especially given the time constraints.

---

### Game Rules

- You have **60 seconds** to deliver as many boxes as possible to a target location.
- **Time decreases** with each delivered box.
- Boxes appear in **random locations**, so luck is a factor too.

**My current high score is 13. Let's see who beats it!**

---

### Pros and Cons

#### Cons:
- Works correctly **only in Windows console**.
- **Freezes during sound playback**.
- **Controls are not very intuitive**.

#### Pros:
- **Unique visual style**
- **Engaging gameplay**

---

### Final Thoughts

I'm happy with the outcome — it feels **competitive and exciting**.  
Hopefully, it catches some attention!
