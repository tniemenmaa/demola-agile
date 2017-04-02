// Mockup data controller
var dummy = 1;
var randomStoryPoints = function() {
    return Math.round(Math.random() * 9) + 1;

}
var MFiles = {
    methods: {
        
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
            /*console.log('getFeatures(' + guid + ')');
            if (guid === '16f218f5-bb95-4e71-b227-8c81b1e5792') {
                console.log('GetUncommitted');
                return MFiles.methods.getUncommittedBacklog();
            }*/
            if (true) {
                return [
                    {
                        backlogId: 'cfeccd08-a327-4fdd-a971-d6ecc76bb6bb',
                        id: '54',
                        name: 'Household work',
                        description: '',
                        collapsed: true,
                        selected: false,
                        userstories: [
                            {
                                id: '425',
                                name: 'Clean up the kitch',
                                description: '',
                                collapsed: true,
                                selected: false,
                                points: randomStoryPoints(),
                                tasks: [
                                    {
                                        id: '1125',
                                        name: 'Take out the trash',
                                        description: ''
                                    },
                                    {
                                        id: '1126',
                                        name: 'Clean the sink',
                                        description: ''
                                    }
                                ]

                            },
                            {
                                id: '511',
                                name: 'House repairs',
                                description: 'Choose what icons would best represent feature, userstory, task, sprint, backlog etc.',
                                collapsed: true,
                                selected: false,
                                points: randomStoryPoints(),
                                tasks: [
                                    {
                                        id: '1301',
                                        name: 'Paint the living room walls',
                                        description: '',
                                        selected: false
                                    },
                                    {
                                        id: '1333',
                                        name: 'Call the mainentenance to fix broken heater',
                                        description: '',
                                        selected: false
                                    }
                                ]
                            },
                        ]
                    },
                    {
                        backlogId: 'cfeccd08-a327-4fdd-a971-d6ecc76bb6bb',
                        id: '63',
                        name: 'Studies',
                        description: 'Work that needs to be done to get our application to talk with M-Files system.',
                        collapsed: true,
                        selected: false,
                        userstories: [
                            {
                                id: '583',
                                name: 'TIE-23506 Web Software Development',
                                description: 'Just basic UI elements with mock data that can be replaced with actual data later. Don\'t focus too much on refining it just basic setup so we can get some feedback from the customer.',
                                collapsed: true,
                                selected: false,
                                points: randomStoryPoints(),
                                tasks: [
                                    {
                                        id: '1894',
                                        name: 'Weekly exercise 1',
                                        description: '',
                                        Selected: false
                                    },
                                    {
                                        id: '1895',
                                        name: 'Weekly exercise 2',
                                        description: '',
                                        Selected: false
                                    },
                                    {
                                        id: '1896',
                                        name: 'Weekly exercise 3',
                                        description: '',
                                        Selected: false
                                    },
                                    {
                                        id: '1897',
                                        name: 'Weekly exercise 4',
                                        description: '',
                                        Selected: false
                                    },
                                    {
                                        id: '1894',
                                        name: 'Course assignment',
                                        description: '',
                                        Selected: false
                                    },
                                    {
                                        id: '1894',
                                        name: 'Study for exam',
                                        description: '',
                                        Selected: false
                                    },
                                ]
                            },
                            {
                                id: '584',
                                name: 'TST-01606 Demola Project Work',
                                description: 'Vault application framework project setup',
                                collapsed: true,
                                selected: false,
                                points: randomStoryPoints(),
                                tasks: [
                                    {
                                        id: '1994',
                                        name: 'Weekly meeting (5.4.2017)',
                                        description: '',
                                        Selected: false
                                    },
                                    {
                                        id: '1995',
                                        name: 'Weekly meeting (19.4.2017)',
                                        description: '',
                                        Selected: false
                                    },
                                    {
                                        id: '1996',
                                        name: 'Partner meeting (10.4.2017)',
                                        description: '',
                                        Selected: false
                                    },
                                    {
                                        id: '2000',
                                        name: 'Demola Pitching event (3.4.2017)',
                                        description: '',
                                        Selected: false
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        backlogId: '16f218f5-bb95-4e71-b227-8c81b1e5792',
                        id: '64',
                        name: 'Finances',
                        description: 'We should use M-Files typography and icons to make our application feel more integrated to M-Files',
                        collapsed: true,
                        selected: false,
                        userstories: [
                            {
                                id: '592',
                                name: 'Bills',
                                description: '',
                                collapsed: true,
                                selected: false,
                                points: randomStoryPoints(),
                                tasks: 
                                    [
                                        {
                                            id: '2022',
                                            name: 'Pay rent',
                                            description: '',
                                            selected: false
                                        },
                                        {
                                            id: '2023',
                                            name: 'Pay electric bill',
                                            description: '',
                                            selected: false
                                        }
                                    ]
                            },
                            {
                                id: '593',
                                name: 'Other',
                                description: '',
                                collapsed: true,
                                selected: false,
                                points: randomStoryPoints(),
                                tasks:
                                    [
                                        {
                                            id: '2024',
                                            name: 'Apply for student aid',
                                            description: '',
                                            selected: false
                                        }
                                    ]
                            }
                        ]
                    },
                {
                    backlogId: '16f218f5-bb95-4e71-b227-8c81b1e5792',
                    id: '65',
                    name: 'Summer job',
                    description: 'We should use M-Files typography and icons to make our application feel more integrated to M-Files',
                    collapsed: true,
                    selected: false,
                    userstories: [
                        {
                            id: '594',
                            name: 'Create kick ass job application',
                            description: '',
                            collapsed: true,
                            selected: false,
                            points: randomStoryPoints(),
                            tasks:
                                [
                                    {
                                        id: '2028',
                                        name: 'Create CV',
                                        description: '',
                                        selected: false
                                    },
                                    {
                                        id: '2029',
                                        name: 'Create a template email for job application',
                                        description: '',
                                        selected: false
                                    },
                                ]

                        },
                        {
                            id: '598',
                            name: 'Events',
                            description: '',
                            collapsed: true,
                            selected: false,
                            points: randomStoryPoints(),
                            tasks:
                                [
                                    {
                                        id: '2030',
                                        name: 'IT Hekuma',
                                        description: '',
                                        selected: false
                                    },
                                    {
                                        id: '2031',
                                        name: 'RecruIT Tampere',
                                        description: '',
                                        selected: false
                                    },
                                ]

                        },
                        {
                            id: '599',
                            name: 'Applications',
                            description: '',
                            collapsed: true,
                            selected: false,
                            points: 0,
                        }
                    ]
                }
                ].filter(function(obj) { return obj.backlogId === guid })
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