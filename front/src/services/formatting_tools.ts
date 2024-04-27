const formatedDate = (date: string): string => {
   const d = new Date(date);
   return `${d.getDate()}/${d.getMonth() + 1}/${d.getFullYear()}`;
};

function formatLengthToTime(length: number): string {
   const minutes = Math.floor(length / 60);
   const seconds = length % 60;

   if (minutes === 0 && seconds === 0) {
      return "00:00";
   }

   if (seconds < 10) {
      return `${minutes}:0${seconds}`;
   }

   if (minutes < 10) {
      return `0${minutes}:${seconds}`;
   }

   if (minutes < 10 && seconds < 10) {
      return `0${minutes}:0${seconds}`;
   }

   return `${minutes}:${seconds}`;
}

function checkIfValidImageURL(url: string): string {
   if (url === null || url === "" || url === undefined || url == "string") {
      return "https://via.placeholder.com/150";
   }
   return url;
}

function checkIfValidAudioURL(url: string): string {
   if (url === null || url === "" || url === undefined || url == "string") {
      return "https://personal-podcast-life-2.s3.amazonaws.com/72d1dc39-139c-46b2-b39b-72fdfadd6596.mp3";
   }
   return url;
}

function checkIfValidTitle(title: string): string {
   if (title === null || title === "" || title === undefined || title == "string") {
      return "No title";
   }
   return title;
}

function checkIfValidDescription(description: string): string {
   if (
      description === null ||
      description === "" ||
      description === undefined ||
      description == "string"
   ) {
      return "No description";
   }
   return description;
}

function checkIfValidVideoURL(url: string): boolean {
   if (url === null || url === "" || url === undefined || url == "string") {
      return false;
   }
   return true;
}

function checkIfVideoOrAudioURL(video: string, audio: string): boolean {
   if (video === null || video === "" || video === undefined || video == "string") {
      if (audio === null || audio === "" || audio === undefined || audio == "string") {
         return true;
      }
   }
   return false;
}

export {
   formatedDate,
   formatLengthToTime,
   checkIfValidImageURL,
   checkIfValidAudioURL,
   checkIfValidTitle,
   checkIfValidDescription,
   checkIfValidVideoURL,
   checkIfVideoOrAudioURL,
};
