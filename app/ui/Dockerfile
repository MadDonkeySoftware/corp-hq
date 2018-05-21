FROM node:9
LABEL Author="MadDonkeySoftware"
LABEL WebSite="http://github.com/MadDonkeySoftware/corp-hq"

EXPOSE 8080

WORKDIR /app

# Copy over the files we need for build / run
COPY . ./

RUN ["npm", "install"]
ENTRYPOINT ["npm", "run", "prod"]