FROM node:slim
ENV NODE_ENV=production

RUN apt-get update && apt-get install -y \
    ffmpeg \ 
    nano \
    zip unzip \
    fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf \
    chromium \
    --no-install-recommends && rm -rf /var/lib/apt/lists/*

WORKDIR /usr/src/app
COPY ["PiketWebApi/WhatAppVenomServer/package.json", "WhatAppVenomServer/"]
COPY ["PiketWebApi/WhatAppVenomServer/package-lock.json", "WhatAppVenomServer/"]
COPY ["PiketWebApi/WhatAppVenomServer/db.js", "WhatAppVenomServer/"]
COPY ["PiketWebApi/WhatAppVenomServer/index.js", "WhatAppVenomServer/"]

WORKDIR /usr/src/app/WhatAppVenomServer
RUN npm install --only=production \
    && npm cache clean --force

COPY . .
EXPOSE 3000
RUN chown -R node /usr/src/app  
USER node
CMD ["node", "index.js"]
