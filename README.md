## Backend service for team Skeptical Beavers

App is deployed at https://morning-tundra-59000.herokuapp.com/

Repo with frontend: https://github.com/WhoAmIRUS/vtb-hackathon

Repo with ml-drawing-based-captcha backend: https://github.com/maftukh/mouse_mapping_ml

To run tests, start docker image with 
```bash
docker build -t skeptical-beavers . && docker run -p 8080:80 skeptical-beavers 
```

## Short backend description:

### 1. SecretApp

After login, user can request a "secret app" from the `/app` endpoint, using JWT token for Authorization.
The page is builded with `npm run build` for each login request with addition of javascript function 
into a random place in the app (place is fixed here as it is only MVP). 
Resulting app is assigned an `appId` and stored. If browser will render produced by `/app` html page, 
this javascript code will call a masked `/challenge` endpoint. 

This endpoint knows what data is expected to come 
(it is mapped from pair of `userName` and `appId` and stored in `Challenges/ChallengeRepository`) 
and is able to check if it is correct. If so, it returns a `App-Auth` key.

Only using a matching pair of `App-Auth` and user JWT token can someone access a `/transaction` endpoint (which is also masked).
If `App-Auth' key is not present or not matched with a userName, response will be 404.

### 2. Obfuscating endpoints

All links in the html and js files that are returned by `/app` are masked. 
They are substituted by a randomly generated Guid. 
This mapping is stored in `Obfuscation/ObfuscatedEndpointsRepository.cs` 
and is restored by `Obfuscation/EndpointDeobfuscationMiddleware` 

## User story:

1. Логин
	- Для входа в систему необходимо ввести логин, пароль и затем нажать кнопку "Войти"
	- Изначально кнопка войти не доступна для нажатия
	- Для того чтобы кнопка войти стала доступна, необходимо успешно пройти валидацию
	- Для прохождения валидации необходимо в специально указаной области нарисовать случайную кривую:
		- Процесс рисования должен занимать не менее 2-х секунд
		- По истечении 2-х секунд шкала прогресса будет полностью заполнены и под областью для рисование появится сообщение "STOP"
	- В случае успешно пройденной валидации под областью для рисование появится сообщение "SUCCESS" и кнопка "Войти" будет доступна для нажатия
	- В случае успешно пройденной валидации под областью для рисование появится сообщение "FAIL", кнопка "Войти" останется не доступна

2. Главная страница
	- После успешной валидации пользователь попадает на Главную страницу
	- На данной странице отображается статус проверки пользователя, с помощью секретной функции (поле App auth status)
	- Если пользователь нажмет на кнопку "SEND" то отправится запрос с транзакцией:
		- В результате успешной проверки пользователя с помощью секретной функции запрос пройдет успешной и в поле Transaction status будет отображено "SUCCESSFULL"
		- В результате не успешной проверки пользователя с помощью секретной функции запрос пройдет не успешной и в поле Transaction status будет отображено "FAILURE"
