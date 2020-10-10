FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/Skeptical.Beavers.Backend/Skeptical.Beavers.Backend.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY src/Skeptical.Beavers.Backend ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
LABEL maintainer="Nikita Chizhov <nik96a@gmail.com>"

# Install npm
RUN apt-get update
RUN apt-get -y install curl gnupg
RUN curl -sL https://deb.nodesource.com/setup_12.x | bash -
RUN apt-get -y install nodejs

COPY ./front /front

# install node modules
WORKDIR /front
RUN npm install

RUN mkdir /apps

# copy asp net core backend
WORKDIR /app
COPY --from=build-env /app/out .
CMD ["dotnet", "Skeptical.Beavers.Backend.dll"]


