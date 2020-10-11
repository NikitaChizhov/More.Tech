## Backend service for team Skeptical Beavers

Repo with frontend: https://github.com/WhoAmIRUS/vtb-hackathon
Repo with ml-drawing-based-captcha backend: https://github.com/maftukh/mouse_mapping_ml

To run tests, start docker image with 
```bash
docker build -t skeptical-beavers . && docker run -p 8080:80 skeptical-beavers 
```