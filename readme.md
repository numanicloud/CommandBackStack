# CommandBackStack

後戻りのある手続き型プログラミングを実現します。

## 例1

数値入力を3回受け付けて、その3つの値の和を表示します。ただし、ユーザーは `cancel` と入力することによって直前に入力した数値を取り消し、入力しなおすことができます。

```csharp
using System;
using System.Threading.Tasks;
using Numani.CommandStack;
using Numani.CommandStack.Maybe;
using Numani.CommandStack.TaskMaybe;

// CommandStackクラスを使用して、後戻りのある手続きを構築して、実行
var sum = await CommandStack.Entry((Unit u) => InputIntegerAsync())
	.Then(x => InputIntegerAsync().FMap(p => x + p))
	.Then(y => InputIntegerAsync().FMap(p => y + p))
	.RunAsync(Unit.Id);

Console.WriteLine(sum);

// ユーザー入力を非同期に受け取るメソッド（の、疑似的なやつ）
// Console.Write などを使ってて非同期にした意味がないですが許して
async Task<IMaybe<int>> InputIntegerAsync()
{
	while (true)
	{
		Console.Write(">");
		var input = Console.ReadLine();
		if (input == "cancel")
		{
			return Maybe.Nothing<int>();
		}

		if (int.TryParse(input, out var result))
		{
			return result.Just();
		}
	}
}
```

実行結果は例えば以下のようになります。

```
>1
>2
>cancel
>100
>3
104
```

## 例2

数値入力を2回受け付けて、その和を計算することを3回繰り返します。最後にそうして得られた3つの値の積を計算して表示します。ただし、ユーザーは `cancel` と入力することによって直前に入力した数値を取り消し、入力しなおすことができます。

```csharp
var sumMul = await CommandStack.ForEach(Enumerable.Range(0, 3), it =>
	{
		return it.Then(i => InputIntegerAsync())
			.Then(x => InputIntegerAsync().FMap(p => x + p));
	})
	.Map(list => list.Aggregate((a, b) => a * b).Just())
	.RunAsync(Unit.Id);

Console.WriteLine(sumMul);

// InputIntegerAsync の実装は先ほどと同じ
```

実行結果は以下のようになります。

```
>1
>2
>3
>900
>cancel
>4
>5
>6
231
```

## ライブラリの内容

### CommandStack<TArg, TResult> クラス

このライブラリの核となるクラスです。後戻りのある手続きのコレクションを保持しています。Then, Map, Do メソッドを使ったメソッドチェーンによって、処理の流れを記述するために使用します。

使用可能なメソッド：

- Then
- Map
- Do

### CommandStack クラス

`CommandStack<TArg, TResult>` クラスのインスタンスを作成するのに使用するスタティックメソッドを持っています。

メソッドチェーンの起点となるオブジェクトはここから作成する必要があります。

使用可能なメソッド：

- Entry
- Fail
- ForEach

### IMaybe<T> インターフェース

有効な値が存在するかどうかの情報を管理する役割を担うインターフェースです。Nullableと同様の機能ですが、値型と参照型の区別をせずに利用できます。

値が有効であることを表すレコード型 `Just<T>` と、無効であることを表すレコード型 `Nothing<T>` がこのインターフェースを実装しています。

`Match` メソッドを用いて、値が有効な場合と無効な場合で別々の値へ射影することができます。

### Maybe クラス

`IMaybe<T>` のインスタンスを生成したりするスタティックメソッドを持ちます。

- Just
- Nothing
- FromNullable
- Match

### MaybeMonad, TaskMonad, TaskMaybeMonad クラス

`IMaybe<T>`, `Task<T>`, `Task<IMaybe<T>>` というそれぞれの型をモナド的に扱うための拡張メソッドを持つクラスです。

主に内部的に利用する目的で用意していますが、`CommandStack`クラスの機能を使う時に併用すると便利なことがあります。特に `Task<IMaybe<T>>.FMap` メソッドはよく使うかも。

それぞれの型に対して、以下の拡張メソッドが有効になります：

- FMap
- Join
- Bind