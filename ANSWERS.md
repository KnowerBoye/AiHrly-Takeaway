## Schema question 

### 1. Database Sketch 

```
applications (
  id UUID PK,
  job_id UUID FK,
  name TEXT NOT NULL,
  email TEXT NOT NULL,
  cover_letter TEXT,
  current_stage TEXT,
  created_at TIMESTAMP,
  updated_at TIMESTAMP
)

application_notes (
  id UUID PK,
  application_id UUID FK,
  type TEXT,
  description TEXT,
  created_by UUID FK,
  created_at TIMESTAMP
)

application_stage_history (
  id UUID PK,
  application_id UUID FK,
  from_stage TEXT,
  to_stage TEXT,
  changed_by UUID FK,
  comment TEXT,
  changed_at TIMESTAMP
)

application_scores (
  id UUID PK,
  application_id UUID FK,
  dimension TEXT,
  score INT,
  comment TEXT,
  updated_by UUID FK,
  updated_at TIMESTAMP
)
```

### Indexes

applications 

UNIQUE(email, jobId) => prevents duplicate applications per job 
INDEX(current_stage) => used in filtering applications
INDEX(jobId) => best practice to index foreign keys

application_scores

INDEX(applicationId) => foreign key 
UNIQUE(applicationId, dimension) enforces one score dimension type per application


### Query 
The `GET /api/applications/{id}` endpoint fetches data from 4 separate tables from the EF logs in terminal you can tell the LINQ is translated into one query of Joins and subqueries making one DB round trip everytime



## 2. Scoring design trade-off

**(a)**
- Having three separate endpoint makes more sense if the UI can only update one score at a time. It helps limit accidental wiping of other scores that weren't included in the payload. In case eventually the endpoints evolve and different scores may have different field requirements this makes validation and maintaining code easier

- Having one endpoint may make sense if UI updates them together in one command or if there are rules like all 3 scores must exist or should have values 

- Alternatively we can have an endpoint like `/api/applications/{id}/scores/{score}`. This however only improves code presentation if all 3 endpoints seems to have no different extra logic. 

**(b)**

I will have to drop the Unique(socre , applicationId) constraint on the application score table to allow multiple same score entries for application. 

- Then update my ApplicationProfileResponse DTO to accommodate for the new shape which is having a list of scores ordered by date_created.

- Or add a new `GET /api/applications/{id}/scores/history` endpoint. And return only the lastest entry per dimension in the `GET /api/applications/{id}` endpoint by grouping by dimension and ordering by date_created


## 3. Debugging question

- First I'll try and reproduce the issue. Open the same application in the browser confirm it's still showing screening. 
Check the request that actually fetches the data if any to see if the response payload actually shows screening if yes then it's a backend issue if no frontend bug or browser caching issue.
- Next I'll open the network tab re trigger the action and check the Fetch/XHR tab to see the request. Verify the request body is actually sending the right stage and also verify the response payload is 200 or 204
- Next I'll inspect the server log for that particular application id. was there any received request , errors that triggers rollbacks ? 
- Query the database since is the main source of truth to check the actual value being stored. Given the DB schema I'll check `application_stage_history` table look at timestamps and changed_by. Am I returning history in the right order ? Is there a race condition overwriting values ? And make the necessary update to code and query logic if any. 
- Also if there's any cache layer in the API like Redis. I'll check the cache invalidation strategies and also TTL. 


## 4. Honest self-assessment. 
- **C#** (3) - My experience with C# is not as extensive as compare to other languages I use on the daily. It has been mostly academic , a few console applications and tutorial projects on ASP.NET

- **SQL** (4) - I have more experience writing queries manually than using ORMs. Admittedly there are a lot of advanced SQL techniques / decisions I haven't had the opportunity to implement in actual projects yet to see how they actually perform and that's something I'm looking forward to. 

- **GIT** (4) - Git is a daily developer tool. I have mastered the basic git workflows, encountered issues that required more digging to figure out and I'm still looking forward to more of those. 

- **REST API** (4) - Having built APIs across very different tech stacks. I understand how the underlying principles are independent of the stacks and that they're just standards to be adhered to. 

- **Writing Test** (3) - I have written tests in different languages and frameworks. I understand the underlying principles.I try as much as possible to include tests without overdoing it. I admittedly have little experience with managing tests across multiple collaborators. 




