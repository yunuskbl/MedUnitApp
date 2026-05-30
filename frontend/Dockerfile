# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npx ng build --configuration docker

# Stage 2: Replace hardcoded production API URL with local backend URL
ARG API_URL=http://localhost:8080
RUN find dist -name "*.js" -o -name "*.mjs" | \
    xargs sed -i "s|https://medunitapp.onrender.com|${API_URL}|g"

# Stage 3: Production runtime (Angular SSR)
FROM node:20-alpine AS final
WORKDIR /app

COPY --from=build /app/dist ./dist
COPY --from=build /app/package*.json ./
RUN npm ci --omit=dev

EXPOSE 4000
ENV PORT=4000
CMD ["node", "dist/med-unit-ui-1/server/server.mjs"]
