import Heading from '@/app/components/Heading'
import React from 'react'
import AuctionForm from '../AuctionForm'

export default function Create() {
  return (
    <div className='mx-auto max-w-[75%] shadow p-10 bg-white rounded-lg'>
      <Heading title='Sell you car!' subtitle='Please enter the details of you car' />
      <AuctionForm />
    </div>
  )
}
