// Mockup data controller
var dummy = 1;
var MFiles = {
    methods: {
        // Get all backlogs
        getUncommittedBacklog: function () {
            return [
                {
                    id: '956e00e2-4c84-4841-bf5c-a2d6509d7dcb',
                    name: 'Uncommitted feature 1',
                    description: 'We should use M-Files typography and icons to make our application feel more integrated to M-Files',
                    collapsed: true,
                    selected: false,
                    userstories: [
                        {
                            id: 'cfeccd08-a327-4fdd-a971-d6ecc76bb6bb',
                            name: 'Uncommitted userstory 1',
                            description: '',
                            collapsed: true,
                            selected: false
                        },
                        {
                            id: 'cfeccd08-a327-4fdd-a971-d6ecc76bb6bb',
                            name: 'Uncommitted userstory 2',
                            description: '',
                            collapsed: true,
                            selected: false
                        },
                        {
                            id: 'cfeccd08-a327-4fdd-a971-d6ecc76bb6bb',
                            name: 'Uncommitted userstory 3',
                            description: '',
                            collapsed: true,
                            selected: false
                        }
                    ]
                },
                {
                    id: '956e00e2-4c84-4841-bf5c-a2d6509d7dcb',
                    name: 'Uncommitted feature 2',
                    description: 'We should use M-Files typography and icons to make our application feel more integrated to M-Files',
                    collapsed: true,
                    selected: false,
                    userstories: [
                        {
                            id: 'cfeccd08-a327-4fdd-a971-d6ecc76bb6bb',
                            name: 'Uncommitted userstory 4',
                            description: '',
                            collapsed: true,
                            selected: false
                        },
                        {
                            id: 'cfeccd08-a327-4fdd-a971-d6ecc76bb6bb',
                            name: 'Uncommitted userstory 5',
                            description: '',
                            collapsed: true,
                            selected: false
                        },
                        {
                            id: 'cfeccd08-a327-4fdd-a971-d6ecc76bb6bb',
                            name: 'Uncommitted userstory 6',
                            description: '',
                            collapsed: true,
                            selected: false
                        }
                    ]
                }
            ]
        },
        getBacklogs: function () {
            if (dummy < 4) {
                dummy += 2;
                return [
                    {
                        id: 'cfeccd08-a327-4fdd-a971-d6ecc76bb6bb',
                        name: 'Sprint 1',
                        description: 'This is backlog for release 1.0',
                        startDate: '2017-01-01',
                        endDate: '2017-03-31',
                        collapsed: true,
                        selected: false
                    },
                    {
                        id: 'f38053e2-35ec-4e12-97fc-3cff33d7fc01',
                        name: 'Sprint 2',
                        description: 'This is backlog for release 1.1',
                        startDate: '2017-04-01',
                        endDate: '2017-06-30',
                        collapsed: true,
                        selected: false
                    },
                    {
                        id: '9c25e68f-8bcb-4051-8702-bb214fa0d67a',
                        name: 'Sprint 3',
                        description: 'This is backlog for release 1.2',
                        startDate: '2017-07-01',
                        endDate: '2017-09-30',
                        collapsed: true,
                        selected: false
                    },
                    {
                        id: '16f218f5-bb95-4e71-b227-8c81b1e57983',
                        name: 'Sprint 4',
                        description: 'This is backlog for release 1.3',
                        startDate: '2017-10-01',
                        endDate: '2017-12-31',
                        collapsed: true,
                        selected: false
                    },
                ]
            }
            else {
                return [
                    {
                        id: '16f218f5-bb95-4e71-b227-8c81b1e5792',
                        name: 'Prospects',
                        description: 'This is backlog for release 1.3',
                        startDate: '2017-10-01',
                        endDate: '2017-12-31',
                        collapsed: false,
                        selected: false
                    },
                    {
                        id: '16f218f5-bb95-4e71-b227-8c81b1e5793',
                        name: 'Release 1',
                        description: 'This is backlog for release 1.3',
                        startDate: '2017-10-01',
                        endDate: '2017-12-31',
                        collapsed: false,
                        selected: false
                    },
                    {
                        id: '16f218f5-bb95-4e71-b227-8c81b1e5793',
                        name: 'Release 2',
                        description: 'This is backlog for release 1.3',
                        startDate: '2017-10-01',
                        endDate: '2017-12-31',
                        collapsed: false,
                        selected: false
                    }

                ]
            }
        },

        // Get features for backlog

        getFeatures: function (guid) {
            console.log('getFeatures(' + guid + ')');
            if (guid === '16f218f5-bb95-4e71-b227-8c81b1e5792') {
                console.log('GetUncommitted');
                return MFiles.methods.getUncommittedBacklog();
            }
            else {
                return [
                    {
                        id: '956e00e2-4c84-4841-bf5c-a2d6509d7dcb',
                        name: 'M-Files look and feel',
                        description: 'We should use M-Files typography and icons to make our application feel more integrated to M-Files',
                        collapsed: true,
                        selected: false,
                        userstories: [
                            {
                                id: '284fdf9c-ee8c-4741-922c-dfa0588234f5',
                                name: 'Get available icons from Mikko',
                                description: 'Get M-Files icon set as JPEG or PNG format so we can use same icon set for our application.',
                                collapsed: true,
                                selected: false,

                            },
                            {
                                id: '996f69a2-fe6a-435d-9b23-18ac938d052b',
                                name: 'Choose icons for objects',
                                description: 'Choose what icons would best represent feature, userstory, task, sprint, backlog etc.',
                                collapsed: true,
                                selected: false,
                            },
                            {
                                id: 'f5c1518a-aab9-4445-8ffd-7c8ba7337f5c',
                                name: 'Gather basic details of M-Files UI to make our UI similar',
                                description: '',
                                collapsed: true,
                                selected: false,
                            }
                        ]
                    },
                    {
                        id: 'a2d4c7a1-f58f-45c7-9f74-7102830aa684',
                        name: 'Attach UI to M-Files Vault Application',
                        description: 'Work that needs to be done to get our application to talk with M-Files system.',
                        collapsed: true,
                        selected: false,
                        userstories: [
                            {
                                id: '68f4ab4f-aed5-47a5-bd50-2ef05baed2c4',
                                name: 'Create initial UI with mock data',
                                description: 'Just basic UI elements with mock data that can be replaced with actual data later. Don\'t focus too much on refining it just basic setup so we can get some feedback from the customer.',
                                collapsed: true,
                                selected: false
                            },
                            {
                                id: '00cd9ae5-caa6-4662-92d9-73b18025dc5c',
                                name: 'VAF Setup',
                                description: 'Vault application framework project setup',
                                collapsed: true,
                                selected: false
                            }
                        ]
                    }
                ]
            }
        }

    }
}


/*


5416db02-5158-4232-8894-6062ed77bb9e
9021f6b2-9ab9-46a0-ad83-827f7534d479
2d5922fb-2432-4bb0-ad11-88fa67e6b04f
1df03296-f088-4722-89ef-28840db50269
90b5964e-0c39-4a13-a92a-a765e309f67e
70485baf-2201-4bd1-91fe-d98787b22795
cc210b67-dda8-4e72-a155-71da26aaa416
420fa6ad-8d07-427a-ba5b-daf7a827805e
967b0c54-56a3-43fe-a31d-1a94343a7c51
*/