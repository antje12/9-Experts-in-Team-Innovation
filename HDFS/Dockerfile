# Start from the Hive image
FROM bde2020/hive:2.3.2-postgresql-metastore

# Add a script to initialize the schema
ADD init-schema.sh /init-schema.sh
RUN chmod +x /init-schema.sh

# Run the schema initialization script on container startup
ENTRYPOINT ["/init-schema.sh"]
