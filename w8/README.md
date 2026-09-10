# Week 8 — Performance Optimization — Sprint 3

Week 8 focused on improving the performance of both capstone APIs using query optimization, Redis caching, and database indexing.

## [Day 1 — Sprint 3 Planning and N+1 Diagnosis](./Day%201/README.md)

The week started by enabling EF Core query logging and adding realistic data volumes to both projects. The main goal was to identify inefficient database queries.

Two summary endpoints showed the N+1 problem:

* Task API: 25 queries, 202ms
* Cardiac API: 21 queries, 253ms

These results became the main optimization target for Day 2.

## [Day 2 — Query Optimization with Projection](./Day%202/README.md)

The N+1 loops were replaced with EF Core projection queries, reducing the number of database queries to one.

* Task API: 25 → 1 query, 202ms → 62ms
* Cardiac API: 21 → 1 query, 253ms → 56ms

`GET /api/tasks` was also optimized by replacing `Include` with projection.

## [Day 3 — Redis Caching](./Day%203/README.md)

Redis caching was added using `IDistributedCache` and the cache-aside pattern.

The first request loads the data from the database and stores it in Redis, while later requests use the cached result. Cache invalidation was also added after relevant write operations.

Redis was run locally using Docker.

## [Day 4 — Database Indexing and Performance Profiling](./Day%204/README.md)

Database indexes were added based on the filtering and sorting patterns used by both APIs.

SQL Server `STATISTICS IO` and `STATISTICS TIME` were used to verify the improvements.

* Task API: 4 logical reads, 3ms
* Cardiac API: 4 logical reads, 10ms

## [Day 5 — Sprint Review, Benchmark Demo, and Retrospective](./Day%205/README.md)

The Sprint 3 work was reviewed and the performance improvements, Redis caching, and cache invalidation were verified.

A process issue was identified: the work was committed directly to `main` instead of using a dedicated feature branch and pull request. This was added to the Sprint 4 backlog.

The main Sprint 4 action is to add regression tests ensuring that the summary endpoints continue using exactly one database query.

## Week 8 Result

By the end of Sprint 3, both APIs had improved through:

* N+1 query elimination
* EF Core projection
* Redis caching
* Cache invalidation
* Database indexing
* SQL Server performance profiling

Sprint 4 will focus on testing, documentation, and deployment.
