/Games - создане игры, Post - запрос

Games/{id}- возвращает игру по id, Get - запрос

/Games/{id}/moves

body: 
{
  "gameId": "019855b5-d488-7ff1-8423-3cdf6d8e2da1",
  "player": "O" или "X",
  "row": 3,
  "column": 0
} - совершить ход , Post - запрос

/Health - отвечает 200 Ok, Get - запрос

