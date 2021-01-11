# V_Project
프로젝트 설명은 아래 [링크](#1)를 통해 영상으로 확인할 수 있고, 코드와 같은 부가설명은 [About Dev](#2) 부분을 참고해주세요.<br>
<br>

### About Project.:two_men_holding_hands:
Irrational Games에서 개발한 택티컬 슈팅 게임 SWAT4를 모작한 프로젝트입니다.<br>
<br>

### Video.:video_camera: <div id="1">이미지를 클릭해주세요.</div>
[![시연영상](https://img.youtube.com/vi/TNQ0OKnjaWw/0.jpg)](https://www.youtube.com/watch?v=TNQ0OKnjaWw)
<br>
<br>

### About Dev.:nut_and_bolt: <div id="2"></div>
<details>
<summary>Object Pool 접기/펼치기</summary>
<div markdown="1">

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> where T : class
{
    int m_count; //몇개를 만들것인가
    int m_max; //최대갯수 지정
    int m_cnt = 0; //현재갯수 카운트
    public delegate T Func(); //무엇을 만들것인가, 해당 T를 생성해서 반환
    Func CreateFunc;
    Queue<T> m_objectPool;

    public int Count { get { return m_objectPool.Count; } }

    public GameObjectPool(int count, int max, Func createFunc)
    {
        m_count = count;
        m_max = max;
        CreateFunc = createFunc; //파라메터로 넘겨준 함수를 받아옴

        m_objectPool = new Queue<T>(count); //count만큼 미리 자리를 잡아놓음
        Allocate();
    }

    public void Allocate()
    {
        //갯수만큼 생성해서 큐에 집어넣음
        for (int i = 0; i < m_count; i++)
        {
            m_objectPool.Enqueue(CreateFunc()); //넘겨준 함수를 통해 T 객체를 생성하고 큐에 넣어준다
            m_cnt++; //현재갯수++
        }
    }

    public T Peek()
    {
        return m_objectPool.Peek();
    }

    public T Get()
    {
        if (m_objectPool.Count > 0)
        {
            return m_objectPool.Dequeue();
        }
        //만약 큐에있는 모든 객체를 다 써버려서 비어있다면, 다시 메모리를 잡아서 생성
        else
        {
            if (m_cnt >= m_max) //최대갯수 이상으로는 새로 할당하지 않음
            {
                return null;
            }

            m_cnt++;
            m_objectPool.Enqueue(CreateFunc()); //새로 객체를 생성 후 큐에 넣어준다
            return m_objectPool.Dequeue(); //해당 객체 반환
        }
    }

    public void Set(T item)
    {
        m_objectPool.Enqueue(item);
    }
}
```


</div>
</details>

<br>


### Difficult Point.:sweat_smile:
* 
<br>

### Feeling.:pencil:

<br>



메인화면 이미지 출처 https://1freewallpapers.com/point-blank-swat-game/ko
