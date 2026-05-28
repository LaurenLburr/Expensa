# WebsitesAddin – Design Conversation

## Purpose

This file stores the ongoing AI design conversation for the add-in design spec.

## Context

- Project: WebsitesAddin
- Design Spec: D:\Git\CodexExpensa\Extensions\Modules\WebsitesAddin\Docs\Addin_Design_Spec.md

## Conversation

### User – 2026-05-09 15:25

The addin will load the Expensa treee with the websitee

### AI – 2026-05-09 15:25

AI scaffold suggestion:

Project: WebsitesAddin
Assembly: CodexExpensa.Feature.WebsitesAddin
Class: WebsitesAddinModule
Root node: Websites

Add-in to load and manage website nodes in the Expensa tree. Architecture: Addin module + tree provider + service + repository. Key files: AddinModule.cs, TreeProvider.cs, WebsiteService.cs, WebsiteRepository.cs, Models/Website.cs, Forms/WebsiteForm.xaml. Data model: Website(Id, Name, Url, IconPath, IsEnabled, CreatedAt, ModifiedAt). Commands: RefreshWebsites, AddWebsite, EditWebsite, RemoveWebsite, OpenWebsite. Workflow: register module, expose 'Websites' root node, lazy-load children, open form for CRUD, persist to Websites table. Next steps: implement module registration and tree provider, scaffold files, create DB table and repository, wire commands and forms, add unit/integration tests. Recommend updating design spec sections: Purpose, Navigation/Tree Structure, Commands, Forms, Data Model, Database Design, Generated Files, Revision History.

### User – 2026-05-09 15:57

The add-in will create a node Called Websites and then add a child for each website found in the database

### AI – 2026-05-09 15:57

AI scaffold suggestion:

Project: WebsitesAddin
Assembly: CodexExpensa.Feature.WebsitesAddin
Class: WebsitesAddinModule
Root node: Websites

Add-in that creates a 'Websites' root node and adds child nodes for each website from the database.

### System – 2026-05-09 17:15

Design Spec was updated from the current conversation.

### AI – 2026-05-09 18:32

Live AI response wiring is pending. Use this section to capture design notes until the OpenAI conversation service is connected.
\#CSharp
### User – 2026-05-11 09:54

CREATE TABLE Website (
    WebsiteId TEXT    NOT NULL
                      PRIMARY KEY,
    Name      TEXT    NOT NULL,
    Url       TEXT    NOT NULL,
    SortIndex INTEGER NOT NULL
                      DEFAULT 0,
    Notes     TEXT    NULL,
    IsActive  INTEGER NOT NULL
                      DEFAULT 1
);

### User – 2026-05-11 09:54

CREATE TABLE WebQuestion (
    WebQuestionId TEXT    NOT NULL
                          PRIMARY KEY,
    WebsiteId     TEXT    NOT NULL,
    QuestionName  TEXT    NOT NULL,
    QuestionText  TEXT    NOT NULL,
    SortIndex     INTEGER NOT NULL
                          DEFAULT 0,
    IsActive      INTEGER NOT NULL
                          DEFAULT 1,
    FOREIGN KEY (
        WebsiteId
    )
    REFERENCES Website (WebsiteId) 
);
