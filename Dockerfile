FROM lodgify/movies-api:3

# Rename the file
RUN mv /app/amovies-db.json /app/movies-db.json