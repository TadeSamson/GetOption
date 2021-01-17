# GetOption
A graphQL like library to ease the pain of server side - database Query with projection for client side


## Basic Usage

#### Sample 1

```javascript
GetOption<User> userOption = new GetOption<User>();
userOption.SearchOption = new SearchOption<User>();
SearchExpression firstnameExp = new PropertySearchExpression() { Property = nameof(User.Firstname), Operator = "%", Value = "mat" };
SearchExpression dobExp = new PropertySearchExpression() { Property = nameof(User.DOB), Operator = ">", Value = "1999-01-17T10:30:40.808+00:00" };
userOption.SearchOption.Expression = new BinarySearchExpression<User>() { LeftSearch = firstnameExp, BinaryOperator = "&&", RightSearch = dobExp };
var users = await userRepository.GetEntitiesAsync(userOption);
```



#### Sample 2
```javascript
var userIds= new List<string>(){"abc","xyz","kcf","mmt","gte"};
GetOption<User> userOption = new GetOption<User>();
userOption.SearchOption = new SearchOption<OrganizationSet>();
string userIdQuery = "";
userIds.ForEach(uid => userIdQuery += $"{nameof(User.Id)}={uid} ||");
userIdQuery = userIdQuery.Trim().Trim('|').Trim('|');
userOption.SearchOption = new SearchOption<User>();
userOption.SearchOption.Expression = SearchOption<User>.DeserializeSearchExpression(userIdQuery);
List<User> users = await userRepository.GetEntitiesAsync(userOption);
```

For information on the motivation and other uses such as for loading specific properties, for sorting, for pagination, see my developing [Medium post](https://tadesamson.medium.com/facebook-graphql-vs-getoption-f3c9e826723b?sk=3c7c9ae117052822d6b366e56f16cdfa).
