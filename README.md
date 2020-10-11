## Backend service for team Skeptical Beavers

App is deployed at https://morning-tundra-59000.herokuapp.com/

Repo with frontend: https://github.com/WhoAmIRUS/vtb-hackathon

Repo with ml-drawing-based-captcha backend: https://github.com/maftukh/mouse_mapping_ml

To run tests, start docker image with 
```bash
docker build -t skeptical-beavers . && docker run -p 8080:80 skeptical-beavers 
```

Short description:

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
