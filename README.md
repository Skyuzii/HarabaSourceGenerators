# <a href="https://habr.com/ru/post/549394/">HarabaSourceGenerators</a>
[![Nuget](https://img.shields.io/nuget/v/HarabaSourceGenerators.Generators)](https://www.nuget.org/packages/HarabaSourceGenerators.Generators/)
[![Nuget](https://img.shields.io/nuget/dt/HarabaSourceGenerators.Generators?color=green)](https://www.nuget.org/packages/HarabaSourceGenerators.Generators/)

Все мы привыкли инжектить кучу зависимостей в класс и инициализировать их в конструкторе.<br>
На выходе обычно получаем что-то типа этого:<br><br>
![alt text](https://image.prntscr.com/image/fQu_o7pGRd2iVGxsoldzZw.png)

Пора с этим кончать!<br>
Представляю вашему вниманию новый, удобный и элегантный способ:<br><br>
![alt text](https://image.prntscr.com/image/pKnVAJnYSLqlGTy5ZgfmYw.png)

А что, если лень указывать для каждой зависимости атрибут Inject?<br>
Не проблема, можно указать атрибут Inject для всего класса.<br> В таком случае будут браться все приватные поля с модификатором readonly:<br><br>
![alt text](https://image.prntscr.com/image/WH_VuE70SxGT-xqSvu_vhg.png)

Отлично. Но что, если есть поле, которое нужно не для инжекта?<br>
Указываем для такого поля атрибут InjectIgnore:<br><br>
![alt text](https://image.prntscr.com/image/I1eSHDglSJewrUg7xlrjYw.png)
