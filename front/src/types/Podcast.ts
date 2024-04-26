type Podcast = {
   id: number;
   title: string;
   description: string;
   categoryId: number;
   tags: string;
   posterImg: string;
   audioFileUrl: string;
   videoFileUrl: string;
   publisherId: number;
   createdDate: string;
   lastUpdate: string | null;
};

export default Podcast;
